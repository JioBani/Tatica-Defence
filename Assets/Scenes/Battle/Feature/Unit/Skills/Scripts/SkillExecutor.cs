using Common.Data.Skills.SkillDefinitions;
using Common.Scripts.StateBase;
using Common.Scripts.Timers;
using Scenes.Battle.Feature.Rounds;
using Scenes.Battle.Feature.Rounds.Phases;
using Scenes.Battle.Feature.Unit.Skills.Contexts;
using Scenes.Battle.Feature.Units.Attackers;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Skills
{
    // 스킬 실행을 담당하는 컴포넌트
    public class SkillExecutor : MonoBehaviour
    {
        [SerializeField] private Units.Unit unit; // 이 스킬 실행기를 가진 유닛 참조
        [SerializeField] private Attacker attacker; // 공격자(타겟을 가진 컴포넌트)
        private SkillDefinitionData _skillData; // 스킬 정의 데이터
        private ISkill _skill; // 실제 스킬 인스턴스
        private float _coolTime; // 스킬의 쿨타임(초)
        private Timer _skillTimer; // 쿨타임 타이머
        private bool _isSkillReady = false; // 스킬 사용 가능 여부

        private void Awake()
        {
            // 컴포넌트 초기화: 같은 게임오브젝트의 Unit 컴포넌트를 가져옴
            unit = GetComponent<Units.Unit>();
        }

        private void OnEnable()
        {
            // 유닛 스폰 이벤트 등록
            unit.OnSpawnEvent += OnSpawn;
            
            // 전투 페이즈 진입 이벤트 등록: 전투 시작 시 스킬을 준비 상태로 설정
            RoundManager.Instance
                .GetStateBase(PhaseType.Combat)
                .Event
                .Add(StateBaseEventType.Enter, OnEnterCombat);
            
            // 전투 페이즈 진입
            RoundManager.Instance
                .GetStateBase(PhaseType.Combat)
                .Event
                .Add(StateBaseEventType.Exit, OnExitCombat);
        }

        private void OnDisable()
        {
            // 이벤트 해제 정리
            unit.OnSpawnEvent -= OnSpawn;
            
            RoundManager.Instance
                .GetStateBase(PhaseType.Combat)
                .Event
                .Remove(StateBaseEventType.Enter, OnEnterCombat);
            
            RoundManager.Instance
                .GetStateBase(PhaseType.Combat)
                .Event
                .Remove(StateBaseEventType.Enter, OnExitCombat);
        }
        
        private void Update()
        {
            // 스킬이 준비됐고 공격 대상이 있을 때 실행 가능 여부를 검사하여 실행
            if (_isSkillReady && attacker?.Victim)
            {
                if (_skill.CanExecute(new CanExecuteContext(attacker.Victim)))
                {
                    Execute();    
                }
            }
        }

        private void OnSpawn(Units.Unit _)
        {
            // 유닛 스폰 시 초기화 수행
            Initialize();
        }

        private void Initialize()
        {
            // 스킬 데이터와 인스턴스 초기화, 타이머 생성
            _skillData = unit.UnitLoadOutData.Skill;
            _coolTime = _skillData.CoolTime;
            _skill = SkillFactory.Instance.CreateSkill(_skillData);
            _skill.Initialize(new InitializeContext(unit));

            _skillTimer = TimerManager.Instance.Make(_coolTime, SetSkillReady);
            _isSkillReady = true; // 초기에는 사용 가능 상태로 설정
        }

        private void OnEnterCombat(PhaseType _, StateBaseEventType __)
        {
            // 전투 시작 시 스킬을 사용 가능으로 만들고 타이머 시작
            _isSkillReady = true;
            _skillTimer.Start();
        }

        private void OnExitCombat(PhaseType _, StateBaseEventType __)
        {
            // 전투 종료 시 타이머 정지(스킬 비활성화 등 추가 로직 필요 시 여기에 추가)
            _skillTimer.Stop();
        }

        private void SetSkillReady(float _)
        {
            // 타이머 콜백: 스킬을 사용 가능 상태로 설정
            _isSkillReady = true;
        }

        private void Execute()
        {
            // 스킬 실행 처리: 준비 상태 해제, Execute 호출, 타이머 재시작
            _isSkillReady = false;
            _skill.Execute(new ExecuteContext(attacker,attacker.Victim));
            _skillTimer.Restart();
        }
    }
}