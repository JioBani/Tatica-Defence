using System;
using System.Collections.Generic;
using Common.Data.Units.UnitStatsByLevel;
using Common.Scripts.Rxs;
using UnityEngine;

namespace Scenes.Battle.Feature.Units.UnitStats.UnitStatSheets
{
    /// <summary>성급 또는 강화 단계 변경 시 전달되는 정보.</summary>
    public readonly struct GradeChangedInfo
    {
        public readonly int Star;
        public readonly int Reinforcement;

        public GradeChangedInfo(int star, int reinforcement)
        {
            Star = star;
            Reinforcement = reinforcement;
        }
    }

    public class UnitStatSheet
    {
        private UnitStatsByLevelData _data;

        /// <summary>현재 성급. 합성(승급) 시 증가한다.</summary>
        public int Star { get; private set; } = 1;

        /// <summary>현재 강화 단계. 강화 합성 시 증가한다. 기본 승급 시 0으로 리셋된다.</summary>
        public int Reinforcement { get; private set; }

        /// <summary>능력치 종류별 단위·범위·기본값·합산 방식의 단일 진실.</summary>
        private readonly UnitStatMetadataCatalog _catalog = new();

        // ── 10종 능력치 (수정자 지원, 단위·범위 강제는 메타데이터 주입으로 수행) ──
        public readonly UnitStat MaxHealth;
        public readonly UnitStat Attack;
        public readonly UnitStat Defense;
        public readonly UnitStat AttackSpeed;
        public readonly UnitStat AttackRange;
        public readonly UnitStat MoveSpeed;
        public readonly UnitStat CriticalChance;
        public readonly UnitStat CriticalDamageMultiplier;
        public readonly UnitStat CooldownReduction;
        public readonly UnitStat DamageDealtIncrease;

        // ── 현재 체력 (런타임 값, 수정자 대상 아님) ──
        public readonly RxValue<float> Health = new(0f);

        public UnitStatSheet()
        {
            // 각 능력치를 자기 종류의 메타데이터(단위·범위·기본값·합산 방식)와 함께 생성한다.
            MaxHealth = new UnitStat(_catalog.Get(UnitStatKind.MaxHealth));
            Attack = new UnitStat(_catalog.Get(UnitStatKind.Attack));
            Defense = new UnitStat(_catalog.Get(UnitStatKind.Defense));
            AttackSpeed = new UnitStat(_catalog.Get(UnitStatKind.AttackSpeed));
            AttackRange = new UnitStat(_catalog.Get(UnitStatKind.AttackRange));
            MoveSpeed = new UnitStat(_catalog.Get(UnitStatKind.MoveSpeed));
            CriticalChance = new UnitStat(_catalog.Get(UnitStatKind.CriticalChance));
            CriticalDamageMultiplier = new UnitStat(_catalog.Get(UnitStatKind.CriticalDamageMultiplier));
            CooldownReduction = new UnitStat(_catalog.Get(UnitStatKind.CooldownReduction));
            DamageDealtIncrease = new UnitStat(_catalog.Get(UnitStatKind.DamageDealtIncrease));
        }

        /// <summary>
        /// 현재 체력을 설정한다. 0 ~ MaxHealth 범위로 clamp된다.
        /// </summary>
        public void SetCurrentHealth(float value)
        {
            Health.Value = Mathf.Clamp(value, 0f, MaxHealth.CurrentValue);
        }

        /// <summary>성급 또는 강화 단계가 변경되었을 때 발생한다.</summary>
        public event Action<GradeChangedInfo> OnGradeChanged;

        public void Init(UnitStatsByLevelData data, int star = 1, int reinforcement = 0)
        {
            _data = data;
            Star = star;
            Reinforcement = reinforcement;

            int effectiveStar = star + reinforcement;
            foreach (var (kind, stat) in Enumerate())
            {
                stat.ClearModifiers();
                stat.SetBaseValue(ResolveBaseValue(data, kind, effectiveStar));
            }

            // 중복 등록 방지: 기존 핸들러를 해제한 뒤 재등록한다
            MaxHealth.OnChange -= OnMaxHealthChanged;

            // 현재 체력 = 최대 체력
            SetCurrentHealth(MaxHealth.CurrentValue);

            // MaxHealth 변경 시 현재 체력 상한 보정
            MaxHealth.OnChange += OnMaxHealthChanged;
        }

        private void OnMaxHealthChanged(float newMax)
        {
            if (Health.Value > newMax) SetCurrentHealth(newMax);
        }

        public UnitStat Get(UnitStatKind kind) => kind switch
        {
            UnitStatKind.MaxHealth                => MaxHealth,
            UnitStatKind.Attack                   => Attack,
            UnitStatKind.Defense                  => Defense,
            UnitStatKind.AttackSpeed              => AttackSpeed,
            UnitStatKind.AttackRange              => AttackRange,
            UnitStatKind.MoveSpeed                => MoveSpeed,
            UnitStatKind.CriticalChance           => CriticalChance,
            UnitStatKind.CriticalDamageMultiplier => CriticalDamageMultiplier,
            UnitStatKind.CooldownReduction        => CooldownReduction,
            UnitStatKind.DamageDealtIncrease      => DamageDealtIncrease,
            _ => null
        };

        public IEnumerable<(UnitStatKind kind, UnitStat stat)> Enumerate()
        {
            yield return (UnitStatKind.MaxHealth,                MaxHealth);
            yield return (UnitStatKind.Attack,                   Attack);
            yield return (UnitStatKind.Defense,                  Defense);
            yield return (UnitStatKind.AttackSpeed,              AttackSpeed);
            yield return (UnitStatKind.AttackRange,              AttackRange);
            yield return (UnitStatKind.MoveSpeed,                MoveSpeed);
            yield return (UnitStatKind.CriticalChance,           CriticalChance);
            yield return (UnitStatKind.CriticalDamageMultiplier, CriticalDamageMultiplier);
            yield return (UnitStatKind.CooldownReduction,        CooldownReduction);
            yield return (UnitStatKind.DamageDealtIncrease,      DamageDealtIncrease);
        }

        /// <summary>
        /// 현재 체력을 최대 체력으로 회복한다. 라운드 종료 시 사용한다.
        /// </summary>
        public void RecoverFullHealth()
        {
            SetCurrentHealth(MaxHealth.CurrentValue);
        }

        /// <summary>성급을 1 올리고 기본 스탯을 갱신한다. 기본 합성(승급) 시 사용한다.</summary>
        public void UpgradeStar()
        {
            Star++;
            Reinforcement = 0;
            RefreshBaseStats();
            OnGradeChanged?.Invoke(new GradeChangedInfo(Star, Reinforcement));
        }

        /// <summary>강화 단계를 올리고 기본 스탯을 갱신한다. 강화 합성 시 사용한다.</summary>
        public void Reinforce(int amount = 1)
        {
            Reinforcement += amount;
            RefreshBaseStats();
            OnGradeChanged?.Invoke(new GradeChangedInfo(Star, Reinforcement));
        }

        /// <summary>기존 수정자를 보존한 채 기본값만 새 성급 기준으로 갱신한다.</summary>
        private void RefreshBaseStats()
        {
            int effectiveStar = Star + Reinforcement;
            foreach (var (kind, stat) in Enumerate())
            {
                stat.SetBaseValue(ResolveBaseValue(_data, kind, effectiveStar));
            }

            SetCurrentHealth(MaxHealth.CurrentValue);
        }

        /// <summary>성급 기준값을 조회하되, 자산에 미입력이면 메타데이터 기본값으로 대체한다.</summary>
        private float ResolveBaseValue(UnitStatsByLevelData data, UnitStatKind kind, int effectiveStar)
        {
            // 미입력(빈 record)을 0으로 떨어뜨리지 않고 단위 정의의 기본값을 쓴다(무력화 방지 — 예: 치명타 배수 0 → 데미지 0).
            return data.HasStat(kind)
                ? data.GetStat(kind, effectiveStar)
                : _catalog.Get(kind).Default;
        }
    }
}
