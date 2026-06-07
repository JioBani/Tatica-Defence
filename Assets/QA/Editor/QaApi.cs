using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Common.Data.Summoners.SummonerDefinitions;
using Common.Data.Synergies;
using Common.Scripts.Draggable;
using Scenes.Battle.Feature.Markets;
using Scenes.Battle.Feature.Rounds;
using Scenes.Battle.Feature.Sells;
using Scenes.Battle.Feature.Synergy;
using Scenes.Battle.Feature.Ui.Markets;
using Scenes.Battle.Feature.Unit.Defenders;
using Scenes.Battle.Feature.Unit.Summoners;
using Scenes.Battle.Feature.Units;
using Scenes.Battle.Feature.WaitingAreas;
using UnityEngine;
using UnityEngine.UI;

namespace QA.Editor
{
    // ─────────────────────────────────────────────
    // QaApi: QA 자동화의 파사드(통로 독립 순수 C# 로직층). qa-spec.json 의 커맨드와 1:1 메서드.
    //   - 통로 독립: MCP 의존(JObject) 없음. 어댑팅·검증은 QaDispatch 가 스펙 기준으로 한다.
    //   - 단방향 의존(QA→게임): 게임 매니저는 SceneSingleton.Instance / Find 로 접근. 게임은 QA를 모른다.
    //   - 에디터 전용(Assembly-CSharp-Editor) → static 허용, 빌드 자연 배제.
    // 새 커맨드 추가: 여기 메서드 추가 + qa-spec.json 에 등재(드리프트 검증기가 정합 강제). 레시피 정본: Assets/QA/README.md.
    //
    // 구현 범위: S1(관측 6 + 입력 5) + S2(관측 GetSummonerState·GetSynergyState, 입력 Reroll·ToggleScanLock·
    //   IncreasePlacementLimit·MoveUnit·SellUnit) + 시간제어(Pause·Resume·SetTimeScale, timeScale 레버) + 블로킹(Step 프레임 전진,
    //   RunUntil 조건 대기). 관측에 필요한 게임 내부는 게임 매니저에 public read-only 게터만 단방향 추가(QA→게임).
    //   블로킹 커맨드(spec blocking:true, 예: Step·RunUntil)는 async Task<object> 시그니처로 qa_await(별도 async 툴)가 디스패치한다
    //   — 벤더가 typeof(Task).IsAssignableFrom 로 async 등록을 판정하므로 UniTask 직접반환은 금지(동기 오등록). 내부는
    //   await UniTask.Yield(Update)로 매프레임 양보. S3(GetUnitStats·GiveUnit·ForceTerminalSlots)는 미구현(스펙에만 등재).
    // ─────────────────────────────────────────────
    public static class QaApi
    {
        // ── 관측 ──

        /// <summary>전장의 큰 흐름(페이즈·라운드·마나·목숨·배치상한)을 반환한다.</summary>
        public static object GetBattleState()
        {
            RoundManager round = RoundManager.Instance;
            MarketManager market = MarketManager.Instance;
            if (round == null || market == null)
            {
                throw new InvalidOperationException("RoundManager/MarketManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            return new Dictionary<string, object>
            {
                // 실제 게임 페이즈를 그대로 노출한다(스펙 phase enum = 7종 PhaseType).
                ["phase"] = round.CurrentState.ToString(),
                ["round"] = round.RoundIndex,
                ["livesLeft"] = round.RemainingFailCount,
                ["mana"] = market.Mana.Value,
                ["placementLimit"] = market.DefenderPlacementLimit.Value,
            };
        }

        /// <summary>소환 터미널(상점) 슬롯과 스캔 잠금 상태를 반환한다.</summary>
        public static object GetTerminalState()
        {
            MarketManager market = MarketManager.Instance;
            if (market == null)
            {
                throw new InvalidOperationException("MarketManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            var slots = new List<object>();
            IReadOnlyList<MarketDefenderSlot> current = market.CurrentSlots;
            for (int index = 0; index < current.Count; index++)
            {
                MarketDefenderSlot slot = current[index];
                slots.Add(new Dictionary<string, object>
                {
                    ["index"] = index,
                    ["definitionId"] = slot.UnitLoadOutData.ID.ToString(),
                    ["name"] = slot.UnitLoadOutData.Unit.DisplayName,
                    ["cost"] = slot.UnitLoadOutData.GetCostByStar(slot.Star),
                    ["empty"] = slot.IsSold,
                });
            }

            return new Dictionary<string, object>
            {
                ["slots"] = slots,
                ["scanLocked"] = market.IsScanLocked.Value,
            };
        }

        /// <summary>대기석 점유 수와 대기 중 소환수 목록을 반환한다.</summary>
        public static object GetBenchState()
        {
            List<ExclusiveDropZone2D> areas = WaitingAreaReferences.Instance != null
                ? WaitingAreaReferences.Instance.waitingAreas
                : throw new InvalidOperationException("WaitingAreaReferences 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");

            var units = new List<object>();
            int occupied = 0;
            for (int slot = 0; slot < areas.Count; slot++)
            {
                Draggable2D occupant = areas[slot].occupant;
                if (occupant != null)
                {
                    occupied++;
                    if (occupant.TryGetComponent(out Defender defender))
                    {
                        object position = new Dictionary<string, object> { ["slotNumber"] = slot };
                        units.Add(BuildDefenderDto(defender, position));
                    }
                }
            }

            return new Dictionary<string, object> { ["occupied"] = occupied, ["units"] = units };
        }

        /// <summary>전장 그리드에 배치된 소환수들과 총 배치 수를 반환한다.</summary>
        public static object GetGridState()
        {
            DefenderManager manager = FindDefenderManager();
            List<Defender> battle = manager.GetBattleAreaDefenders();

            DefenderSideSell[] zones = FindBattleZones();
            (List<float> lanes, List<float> columns) axes = BuildGridAxes(zones);

            var units = new List<object>();
            foreach (Defender defender in battle)
            {
                object position = BuildBattleCellPosition(defender, zones, axes.lanes, axes.columns);
                units.Add(BuildDefenderDto(defender, position));
            }

            return new Dictionary<string, object> { ["placedCount"] = battle.Count, ["units"] = units };
        }

        /// <summary>전투 중 소환수·침략자 현황과 결과를 반환한다.</summary>
        public static object GetCombatState()
        {
            DefenderManager defenderManager = FindDefenderManager();
            RoundManager round = RoundManager.Instance;
            RoundAggressorManager aggressorManager = UnityEngine.Object.FindAnyObjectByType<RoundAggressorManager>();
            if (round == null || aggressorManager == null)
            {
                throw new InvalidOperationException("RoundManager/RoundAggressorManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            // 소환수: 전장에 배치된 것(전투 참여) + 격자 위치.
            DefenderSideSell[] zones = FindBattleZones();
            (List<float> lanes, List<float> columns) axes = BuildGridAxes(zones);
            var defenders = new List<object>();
            foreach (Defender defender in defenderManager.GetBattleAreaDefenders())
            {
                object position = BuildBattleCellPosition(defender, zones, axes.lanes, axes.columns);
                defenders.Add(BuildDefenderDto(defender, position));
            }

            // 침략자: 살아있는(활성) 유닛만 통일 Unit 베이스로 조회.
            var aggressors = new List<object>();
            foreach (Scenes.Battle.Feature.Units.Unit aggressor in aggressorManager.Aggressors)
            {
                if (aggressor != null && aggressor.gameObject.activeInHierarchy)
                {
                    aggressors.Add(BuildUnitBaseDto(aggressor, "aggressor"));
                }
            }

            return new Dictionary<string, object>
            {
                ["defenders"] = defenders,
                ["aggressors"] = aggressors,
                ["remaining"] = aggressorManager.RemainingAggressorCount,
                ["result"] = MapResult(round.CurrentState.ToString()),
            };
        }

        /// <summary>각 레인 끝을 지키는 소환술사들의 상태를 반환한다.</summary>
        public static object GetSummonerState()
        {
            SummonerManager manager = SummonerManager.Instance;
            if (manager == null)
            {
                throw new InvalidOperationException("SummonerManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            // 소환술사 레인은 전장 격자 레인 축(월드 Y 오름차순)에 매핑한다 — 소환수 위치 모델과 동일 컨벤션.
            DefenderSideSell[] zones = FindBattleZones();
            (List<float> lanes, List<float> columns) axes = BuildGridAxes(zones);

            var summoners = new List<object>();
            foreach (Summoner summoner in manager.SpawnedSummoners)
            {
                if (summoner != null)
                {
                    summoners.Add(BuildSummonerDto(summoner, axes.lanes));
                }
            }

            return new Dictionary<string, object> { ["summoners"] = summoners };
        }

        /// <summary>시너지(심상 효과) 현황을 반환한다.</summary>
        public static object GetSynergyState()
        {
            SynergyManager manager = SynergyManager.Instance;
            if (manager == null)
            {
                throw new InvalidOperationException("SynergyManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            var synergies = new List<object>();
            foreach (KeyValuePair<SynergyDefinitionData, SynergyActivation> entry in manager.SynergyActivations)
            {
                synergies.Add(BuildSynergyDto(entry.Key, entry.Value));
            }

            return new Dictionary<string, object> { ["synergies"] = synergies };
        }

        /// <summary>현재 게임 화면(월드+UI 최종 프레임)을 파일로 저장하고 경로를 반환한다.</summary>
        public static object Screenshot()
        {
            // 결정적 경로 + 타임스탬프 파일명. ScreenCapture 는 "다음 프레임"에 기록되므로 경로만 즉시 반환한다.
            string root = Directory.GetParent(Application.dataPath)?.FullName ?? Application.dataPath;
            string directory = Path.Combine(root, "QA_Screenshots");
            Directory.CreateDirectory(directory);
            string fileName = $"qa_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png";
            string path = Path.Combine(directory, fileName);

            ScreenCapture.CaptureScreenshot(path);
            return new Dictionary<string, object> { ["path"] = path };
        }

        // ── 입력 ──

        /// <summary>소환 터미널 slotIndex 번 후보의 소환 버튼을 탭한다.</summary>
        public static object SummonUnit(int slotIndex)
        {
            DefenderSlot[] slots = UnityEngine.Object.FindObjectsByType<DefenderSlot>(FindObjectsInactive.Include);
            DefenderSlot slot = slots.FirstOrDefault(s => s.Index == slotIndex);
            if (slot == null)
            {
                throw new InvalidOperationException($"터미널 슬롯 {slotIndex} 의 UI(DefenderSlot)를 찾지 못했습니다.");
            }

            // 소환 성공 여부는 구매로 대기석에 소환수가 추가되었는지(Defenders 수 증가)로 판정한다.
            DefenderManager manager = FindDefenderManager();
            int before = manager.Defenders.Count;
            slot.OnClick(); // 슬롯 카드 버튼의 클릭 핸들러(실제 입력 경로)를 발동한다.
            bool summoned = manager.Defenders.Count > before;

            return new Dictionary<string, object> { ["summoned"] = summoned };
        }

        /// <summary>대기석 소환수를 전장 그리드 칸에 배치한다(드래그 동작, 배치 규칙 준수).</summary>
        public static object PlaceUnit(int unitInstanceId, QaGridPosition position)
        {
            if (position == null)
            {
                throw new ArgumentException("position(lane,column) 인자가 필요합니다.");
            }

            DefenderManager manager = FindDefenderManager();
            Defender defender = manager.Defenders.FirstOrDefault(d => InstanceIdOf(d.gameObject) == unitInstanceId);
            if (defender == null)
            {
                throw new InvalidOperationException($"instanceId {unitInstanceId} 에 해당하는 소환수가 없습니다.");
            }

            DefenderSideSell[] zones = FindBattleZones();
            DefenderSideSell target = FindZoneAtCell(zones, position.lane, position.column);
            if (target == null)
            {
                throw new InvalidOperationException($"전장 격자 칸 (lane={position.lane}, column={position.column}) 이 없습니다.");
            }

            Draggable2D draggable = defender.GetComponent<Draggable2D>();
            if (draggable == null)
            {
                throw new InvalidOperationException("소환수에 Draggable2D 가 없습니다.");
            }

            // 실제 드롭 게이트와 동일하게 배치 규칙(정비 페이즈·배치 상한)을 먼저 검사한다(MoveToDropZone 자체는 규칙 미검사).
            bool placed = target.CanAccept(draggable, draggable.thisDropZone);
            if (placed)
            {
                draggable.MoveToDropZone(target);
            }

            return new Dictionary<string, object> { ["placed"] = placed };
        }

        /// <summary>전투 시작 버튼을 눌러 정비 페이즈를 끝낸다.</summary>
        public static object StartCombat()
        {
            RoundManager round = RoundManager.Instance;
            if (round == null)
            {
                throw new InvalidOperationException("RoundManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            Button startButton = FindButtonByPersistentMethod("SetReady");
            if (startButton == null)
            {
                throw new InvalidOperationException("전투 시작 버튼(onClick → SetReady)을 씬에서 찾지 못했습니다.");
            }

            bool wasMaintenance = round.CurrentState.ToString() == "Maintenance";
            startButton.onClick.Invoke();
            bool started = wasMaintenance && round.CurrentState.ToString() == "Ready";

            return new Dictionary<string, object> { ["started"] = started };
        }

        /// <summary>소환 터미널 UI를 연다.</summary>
        public static object OpenTerminal()
        {
            return SetTerminalOpen(true);
        }

        /// <summary>소환 터미널 UI를 닫는다.</summary>
        public static object CloseTerminal()
        {
            return SetTerminalOpen(false);
        }

        /// <summary>재스캔(상점 새로고침) 버튼을 눌러 소환 후보를 다시 뽑는다.</summary>
        public static object Reroll()
        {
            MarketManager market = MarketManager.Instance;
            if (market == null)
            {
                throw new InvalidOperationException("MarketManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }
            MarketUiManager ui = FindMarketUi();

            // 재스캔은 마나를 소모하므로 마나 감소를 성공 신호로 본다(실제 입력 경로: 재스캔 버튼 핸들러).
            int manaBefore = market.Mana.Value;
            ui.OnClickReroll();
            bool rerolled = market.Mana.Value < manaBefore;

            if (!rerolled)
            {
                Debug.LogWarning($"[QA] Reroll 거부 — available={market.IsMarketAvailable.Value}, mana={manaBefore}/{market.RerollMana.Value}(필요).");
            }

            return new Dictionary<string, object> { ["rerolled"] = rerolled };
        }

        /// <summary>스캔 잠금 버튼을 눌러 자동 재스캔 잠금을 토글한다.</summary>
        public static object ToggleScanLock()
        {
            MarketManager market = MarketManager.Instance;
            if (market == null)
            {
                throw new InvalidOperationException("MarketManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }
            MarketUiManager ui = FindMarketUi();

            bool before = market.IsScanLocked.Value;
            ui.OnClickScanLock();
            bool after = market.IsScanLocked.Value;

            // 토글은 정비 페이즈(상점 가용)에서만 동작한다. 상태가 그대로면 거부된 것이므로 단서를 남긴다.
            if (after == before)
            {
                Debug.LogWarning($"[QA] ToggleScanLock 무효 — available={market.IsMarketAvailable.Value}(정비 페이즈 외 거부).");
            }

            return new Dictionary<string, object> { ["scanLocked"] = after };
        }

        /// <summary>배치 상한 증가(레벨업) 버튼을 눌러 배치 가능 수를 1 늘린다.</summary>
        public static object IncreasePlacementLimit()
        {
            MarketManager market = MarketManager.Instance;
            if (market == null)
            {
                throw new InvalidOperationException("MarketManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }
            MarketUiManager ui = FindMarketUi();

            int limitBefore = market.DefenderPlacementLimit.Value;
            ui.OnClickLevelUp();
            int limitAfter = market.DefenderPlacementLimit.Value;
            bool increased = limitAfter > limitBefore;

            if (!increased)
            {
                Debug.LogWarning($"[QA] IncreasePlacementLimit 거부 — available={market.IsMarketAvailable.Value}, maxLevel={market.IsMaxLevel()}, mana={market.Mana.Value}/{market.LevelUpMana.Value}(필요).");
            }

            return new Dictionary<string, object> { ["increased"] = increased, ["placementLimit"] = limitAfter };
        }

        /// <summary>그리드의 소환수를 다른 칸으로 이동한다(빈 칸=이동, 점유 칸=교환/동일 유닛 합성). 원래 드래그 동작.</summary>
        public static object MoveUnit(QaGridPosition from, QaGridPosition to)
        {
            if (from == null || to == null)
            {
                throw new ArgumentException("from/to(lane,column) 인자가 모두 필요합니다.");
            }

            DefenderSideSell[] zones = FindBattleZones();
            DefenderSideSell sourceZone = FindZoneAtCell(zones, from.lane, from.column);
            DefenderSideSell targetZone = FindZoneAtCell(zones, to.lane, to.column);
            if (sourceZone == null)
            {
                throw new InvalidOperationException($"출발 격자 칸 (lane={from.lane}, column={from.column}) 이 없습니다.");
            }
            if (targetZone == null)
            {
                throw new InvalidOperationException($"도착 격자 칸 (lane={to.lane}, column={to.column}) 이 없습니다.");
            }

            Draggable2D occupant = sourceZone.occupant;
            bool moved;
            if (occupant == null)
            {
                // 출발 칸이 비어 이동 대상이 없으면 단서를 남기고 실패로 본다(조용한 실패 금지).
                Debug.LogWarning($"[QA] MoveUnit 실패 — 출발 칸 (lane={from.lane}, column={from.column}) 에 소환수가 없습니다.");
                moved = false;
            }
            else
            {
                // 실제 드롭 게이트와 동일하게 배치 규칙(정비 페이즈 등)을 먼저 검사한다. 점유 칸이면
                // MoveToDropZone → 존.OnDrop 에서 스왑 또는 동일 유닛 강화 합성이 자동 처리된다.
                moved = targetZone.CanAccept(occupant, occupant.thisDropZone);
                if (moved)
                {
                    occupant.MoveToDropZone(targetZone);
                }
                else
                {
                    Debug.LogWarning($"[QA] MoveUnit 거부 — 도착 칸 (lane={to.lane}, column={to.column}) 배치 규칙 불충족.");
                }
            }

            return new Dictionary<string, object> { ["moved"] = moved };
        }

        /// <summary>소환수를 환원(판매)하여 성급·강화에 따른 마나를 돌려받는다(원래 상점 드래그 동작).</summary>
        public static object SellUnit(int unitInstanceId)
        {
            DefenderManager manager = FindDefenderManager();
            MarketManager market = MarketManager.Instance;
            if (market == null)
            {
                throw new InvalidOperationException("MarketManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            Defender defender = manager.Defenders.FirstOrDefault(d => InstanceIdOf(d.gameObject) == unitInstanceId);
            if (defender == null)
            {
                throw new InvalidOperationException($"instanceId {unitInstanceId} 에 해당하는 소환수가 없습니다.");
            }

            // 판매 입력의 종착점은 MarketManager.Sell 이다. 판매존(DefenderSellZone)은 드래그 위치가 화면 rect 안인지의
            // 히트테스트 게이트라 헤드리스로 재현 불가하므로, 드래그가 도달하는 동일 동작(Sell)을 직접 호출한다.
            int manaBefore = market.Mana.Value;
            market.Sell(defender);
            bool sold = !manager.Defenders.Contains(defender);
            int refund = market.Mana.Value - manaBefore;

            return new Dictionary<string, object> { ["sold"] = sold, ["refund"] = refund };
        }

        // ── 시간 제어 ──
        // 정지 레버는 Time.timeScale 이다. DynamicRepeater(공격 UniTask.Delay)·Timer(Time.deltaTime)·Mover(Rigidbody2D 물리)
        // 모두 스케일시간 기반이라 timeScale=0 이면 실제로 진행이 멈춘다. 에디터 업데이트 루프·MCP 브릿지는 계속 돌아
        // 정지 중에도 QA 관측/제어 호출은 처리된다(EditorApplication.isPaused 가 아닌 timeScale 을 쓰는 이유).

        /// <summary>시뮬레이션을 정지한다(전투·이동·타이머·스킬 멈춤). 정지 중에도 QA 관측은 가능하다.</summary>
        public static object Pause()
        {
            Time.timeScale = 0f;
            return new Dictionary<string, object> { ["paused"] = true, ["timeScale"] = 0f };
        }

        /// <summary>정지를 해제하고 항상 1배속으로 복귀한다(직전 배율이 아님).</summary>
        public static object Resume()
        {
            Time.timeScale = 1f;
            return new Dictionary<string, object> { ["paused"] = false, ["timeScale"] = 1f };
        }

        /// <summary>시뮬레이션 속도 배율을 설정한다(0=정지, 1=기본, &gt;1 가속, &lt;1 감속). 음수는 거부한다.</summary>
        public static object SetTimeScale(float scale)
        {
            // 음수 배율은 잘못된 입력 — 적용하지 않고 거부 응답으로 단서를 남긴다(조용한 실패 금지).
            if (scale < 0f)
            {
                Debug.LogWarning($"[QA] SetTimeScale 거부 — 음수 배율({scale}). 0 이상이어야 합니다.");
                throw new ArgumentOutOfRangeException(nameof(scale), scale, "타임스케일은 0 이상이어야 합니다(음수 거부).");
            }

            Time.timeScale = scale;
            return new Dictionary<string, object> { ["timeScale"] = scale };
        }

        /// <summary>시뮬레이션을 N 프레임 전진시킨 뒤 다시 정지한다(프레임 단위 관측용). 블로킹 커맨드(qa_await 로 디스패치).</summary>
        public static async Task<object> Step(int frames)
        {
            // frames<1 은 잘못된 입력 — 전진 없이 거부하고 단서를 남긴다(조용한 실패 금지).
            if (frames < 1)
            {
                Debug.LogWarning($"[QA] Step 거부 — frames({frames})는 1 이상이어야 합니다.");
                throw new ArgumentOutOfRangeException(nameof(frames), frames, "frames 는 1 이상이어야 합니다.");
            }

            // 정지 상태에서 한 프레임씩 전진한다. timeScale=0 이면 deltaTime 이 흐르지 않아 yield 만으로는 진행이 없으므로,
            // 프레임 경계마다 1↔0 토글로 정확히 한 프레임치 시뮬레이션만 흐르게 한다. 끝나면 정지(0) 상태로 남는다.
            Time.timeScale = 0f;
            for (int frame = 0; frame < frames; frame++)
            {
                Time.timeScale = 1f;
                await UniTask.Yield(PlayerLoopTiming.Update);
                Time.timeScale = 0f;
            }

            return new Dictionary<string, object> { ["paused"] = true, ["steppedFrames"] = frames };
        }

        /// <summary>지정 조건이 충족되거나 타임아웃까지 폴링 대기 후 결과를 반환한다(블로킹 async). 충족 시 freezeOnTrigger 면 정지.</summary>
        public static async Task<object> RunUntil(JObject condition, int timeoutMs = 28000, bool freezeOnTrigger = true)
        {
            // 조건 명세를 매 프레임 평가 probe 로 빌드한다. 미지 type·누락 파라미터·매니저 부재는 여기서 거부(예외→봉투 error).
            Func<object> probe = QaConditionFactory.Build(condition);
            QaWaitEngine.WaitOutcome outcome = await QaWaitEngine.WaitUntil(probe, timeoutMs, freezeOnTrigger);

            // matchedCondition 은 충족 시 probe 가 만든 정보(누구/어느 페이즈), 타임아웃이면 null.
            return new Dictionary<string, object>
            {
                ["triggered"] = outcome.Triggered,
                ["reason"] = outcome.Triggered ? "conditionMet" : "timeout",
                ["matchedCondition"] = outcome.Match,
                ["frozen"] = outcome.Frozen,
            };
        }

        /// <summary>터미널을 원하는 열림 상태로 만든다(현재와 다를 때만 토글 버튼 핸들러 발동).</summary>
        private static object SetTerminalOpen(bool open)
        {
            MarketUiManager ui = FindMarketUi();

            // 토글은 정비 페이즈(상점 이용 가능)에서만 동작한다. 현재 상태와 다를 때만 실제 입력 경로(OnClickToggle)를 탄다.
            if (ui.IsOpen != open)
            {
                ui.OnClickToggle();
            }
            return new Dictionary<string, object> { ["open"] = ui.IsOpen };
        }

        // ── DTO 빌더 ──

        /// <summary>유닛 공통 관측면(Unit 베이스)을 평탄 DTO로 만든다.</summary>
        private static Dictionary<string, object> BuildUnitBaseDto(Scenes.Battle.Feature.Units.Unit unit, string kind)
        {
            string rawState = unit.ActionStateController.CurrentState.ToString();
            var dto = new Dictionary<string, object>
            {
                ["instanceId"] = InstanceIdOf(unit.gameObject),
                ["kind"] = kind,
                ["name"] = unit.UnitLoadOutData != null ? unit.UnitLoadOutData.Unit.DisplayName : unit.name,
                ["definitionId"] = unit.UnitLoadOutData != null ? unit.UnitLoadOutData.ID.ToString() : null,
                ["faction"] = unit.fraction.ToString(),
                ["star"] = unit.StatSheet.Star,
                ["reinforcement"] = unit.StatSheet.Reinforcement,
                ["health"] = unit.StatSheet.Health.Value,
                ["maxHealth"] = unit.StatSheet.MaxHealth.CurrentValue,
                // 실제 행동 상태를 그대로 노출한다(스펙 actionState enum 에 Waiting 포함).
                ["actionState"] = rawState,
                ["alive"] = rawState != "Downed",
                ["worldPosition"] = new Dictionary<string, object>
                {
                    ["x"] = unit.transform.position.x,
                    ["y"] = unit.transform.position.y,
                },
            };
            return dto;
        }

        /// <summary>소환수 DTO(Unit 베이스 + 배치·위치·시너지)를 만든다. position 은 호출자가 영역에 맞게 계산해 넘긴다.</summary>
        private static Dictionary<string, object> BuildDefenderDto(Defender defender, object position)
        {
            Dictionary<string, object> dto = BuildUnitBaseDto(defender, "defender");
            dto["placement"] = defender.Placement.ToString();
            dto["position"] = position;
            dto["synergies"] = defender.Synergies.Select(s => s != null ? s.name : null).ToList();
            return dto;
        }

        /// <summary>소환술사 DTO(Unit 베이스 + 술사정의 + 담당 레인)를 만든다. lane 은 전장 레인 축에 매핑한다.</summary>
        private static Dictionary<string, object> BuildSummonerDto(Summoner summoner, List<float> lanes)
        {
            Dictionary<string, object> dto = BuildUnitBaseDto(summoner, "summoner");

            SummonerDefinitionData definition = summoner.SummonerDefinition;
            if (definition != null)
            {
                dto["summonerDefinition"] = new Dictionary<string, object>
                {
                    ["definitionId"] = definition.ID.ToString(),
                    ["name"] = definition.DisplayName,
                };
            }
            else
            {
                // OnSpawn 캐스팅 실패 등으로 정의가 비어 있으면 단서를 남긴다(조용한 누락 금지).
                Debug.LogWarning($"[QA] 소환술사 '{summoner.name}' 의 SummonerDefinition 이 비어 있습니다.");
                dto["summonerDefinition"] = null;
            }

            dto["lane"] = LaneIndexOfWorldY(summoner.transform.position.y, lanes);
            return dto;
        }

        /// <summary>시너지 1종의 현황 DTO(식별자·이름·카운트·현재/다음 임계치)를 만든다.</summary>
        private static Dictionary<string, object> BuildSynergyDto(SynergyDefinitionData definition, SynergyActivation activation)
        {
            int count = activation.Count;

            // 현재 발동 임계치: 활성 티어의 요구 수(미발동이면 0).
            int activeThreshold = activation.ActiveTier.Value.HasValue
                ? activation.ActiveTier.Value.Value.RequiredCount
                : 0;

            var dto = new Dictionary<string, object>
            {
                ["id"] = definition.Id.ToString(),
                ["name"] = definition.DisplayName,
                ["count"] = count,
                ["activeThreshold"] = activeThreshold,
            };

            // 다음 임계치: 현재 카운트보다 큰 첫 티어 요구 수(티어는 requiredCount 오름차순). 없으면 키 생략.
            if (definition.Tiers != null)
            {
                foreach (SynergyTier tier in definition.Tiers)
                {
                    if (tier.RequiredCount > count)
                    {
                        dto["nextThreshold"] = tier.RequiredCount;
                        break;
                    }
                }
            }

            return dto;
        }

        // ── 전장 격자(4 레인 x 8 열) ──

        /// <summary>전장 배치 드롭존(DefenderSideSell)을 씬에서 수집한다(중앙 수집자 부재 → QA 에디터 한정 Find).</summary>
        private static DefenderSideSell[] FindBattleZones()
        {
            return UnityEngine.Object.FindObjectsByType<DefenderSideSell>(FindObjectsInactive.Include);
        }

        /// <summary>드롭존들의 월드 좌표에서 레인(Y 오름차순)·열(X 오름차순) 축 값을 만든다.</summary>
        private static (List<float> lanes, List<float> columns) BuildGridAxes(DefenderSideSell[] zones)
        {
            List<float> lanes = zones.Select(z => RoundCoord(z.transform.position.y)).Distinct().OrderBy(v => v).ToList();
            List<float> columns = zones.Select(z => RoundCoord(z.transform.position.x)).Distinct().OrderBy(v => v).ToList();
            return (lanes, columns);
        }

        /// <summary>(lane,column) 칸에 해당하는 드롭존을 찾는다(없으면 null).</summary>
        private static DefenderSideSell FindZoneAtCell(DefenderSideSell[] zones, int lane, int column)
        {
            (List<float> lanes, List<float> columns) = BuildGridAxes(zones);
            if (lane < 0 || lane >= lanes.Count || column < 0 || column >= columns.Count)
            {
                return null;
            }
            float targetY = lanes[lane];
            float targetX = columns[column];
            return zones.FirstOrDefault(z =>
                Mathf.Approximately(RoundCoord(z.transform.position.y), targetY) &&
                Mathf.Approximately(RoundCoord(z.transform.position.x), targetX));
        }

        /// <summary>배치된 소환수가 점유한 드롭존의 (lane,column) 위치 DTO를 만든다(미발견 시 빈 객체).</summary>
        private static object BuildBattleCellPosition(Defender defender, DefenderSideSell[] zones, List<float> lanes, List<float> columns)
        {
            Draggable2D draggable = defender.GetComponent<Draggable2D>();
            DefenderSideSell zone = zones.FirstOrDefault(z => z.occupant == draggable);
            var position = new Dictionary<string, object>();
            if (zone != null)
            {
                position["lane"] = lanes.IndexOf(RoundCoord(zone.transform.position.y));
                position["column"] = columns.IndexOf(RoundCoord(zone.transform.position.x));
            }
            return position;
        }

        private static float RoundCoord(float value)
        {
            return Mathf.Round(value * 100f) / 100f;
        }

        /// <summary>월드 Y 를 전장 레인 축에서 가장 가까운 레인 인덱스로 매핑한다(축이 비면 -1).</summary>
        private static int LaneIndexOfWorldY(float worldY, List<float> lanes)
        {
            if (lanes.Count == 0)
            {
                return -1;
            }

            int nearest = 0;
            float nearestDistance = Mathf.Abs(lanes[0] - worldY);
            for (int i = 1; i < lanes.Count; i++)
            {
                float distance = Mathf.Abs(lanes[i] - worldY);
                if (distance < nearestDistance)
                {
                    nearest = i;
                    nearestDistance = distance;
                }
            }
            return nearest;
        }

        // ── 공용 헬퍼 ──

        /// <summary>유닛 인스턴스의 정수 식별자(살아있는 동안 유일). 풀 재사용에도 GO가 파괴되지 않아 안정적이다.</summary>
        private static int InstanceIdOf(GameObject go)
        {
            // GetInstanceID 는 후속 버전에서 GetEntityId 권장이나, 스펙상 정수 instanceId 가 필요해 의도적으로 사용한다.
#pragma warning disable CS0618
            return go.GetInstanceID();
#pragma warning restore CS0618
        }

        /// <summary>씬의 DefenderManager 를 찾는다(SceneSingleton 아님 → Find). 없으면 예외.</summary>
        private static DefenderManager FindDefenderManager()
        {
            DefenderManager manager = UnityEngine.Object.FindAnyObjectByType<DefenderManager>();
            if (manager == null)
            {
                throw new InvalidOperationException("DefenderManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }
            return manager;
        }

        /// <summary>씬의 MarketUiManager 를 찾는다(패널 비종속 입력 핸들러 호출용). 없으면 예외.</summary>
        private static MarketUiManager FindMarketUi()
        {
            MarketUiManager ui = UnityEngine.Object.FindAnyObjectByType<MarketUiManager>();
            if (ui == null)
            {
                throw new InvalidOperationException("MarketUiManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }
            return ui;
        }

        /// <summary>onClick 영속 콜백이 지정 메서드명을 호출하는 Button 을 씬에서 찾는다(없으면 null).</summary>
        private static Button FindButtonByPersistentMethod(string methodName)
        {
            Button[] buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
            foreach (Button button in buttons)
            {
                int count = button.onClick.GetPersistentEventCount();
                for (int i = 0; i < count; i++)
                {
                    if (button.onClick.GetPersistentMethodName(i) == methodName)
                    {
                        return button;
                    }
                }
            }
            return null;
        }

        /// <summary>내부 페이즈에서 전투 결과(InProgress/Victory/Defeat)를 도출한다.</summary>
        private static string MapResult(string phase)
        {
            switch (phase)
            {
                case "BattleWin": return "Victory";
                case "BattleLose": return "Defeat";
                default: return "InProgress";
            }
        }
    }

    /// <summary>PlaceUnit/MoveUnit 의 전장 그리드 위치 인자(qa-spec.json input/GridPosition 대응).</summary>
    public sealed class QaGridPosition
    {
        /// <summary>레인 인덱스(0~3, 월드 Y 오름차순).</summary>
        public int lane { get; set; }

        /// <summary>열 인덱스(0~7, 월드 X 오름차순).</summary>
        public int column { get; set; }
    }
}
