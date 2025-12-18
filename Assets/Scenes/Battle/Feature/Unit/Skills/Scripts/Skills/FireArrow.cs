using Scenes.Battle.Feature.Unit.Attackers.AttackContexts;
using Scenes.Battle.Feature.Unit.Attackers.AttackContexts.Dtos;
using Scenes.Battle.Feature.Unit.Skills.Contexts;
using Scenes.Battle.Feature.Units.Attackables;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Skills.SkillExecutors
{
    public class FireArrow : ISkill
    {
        private Units.Unit _attacker;
        
        public void Initialize(InitializeContext context)
        {
            _attacker = context.Attacker;
        }

        public void Execute(ExecuteContext context)
        {
            // TODO: 기본공격의 attackContext를 사용하는 것이 맞는지
            // attackContext 는 설계 당시 기본공격을 대상으로 설계되었기 떄문에,
            // attacker 등을 인수로 받고 있음
            // attacker 는 스킬 사용시 필수가 아님(attacker 가 없는 유닛이 스킬을 사용 할 수도 있음)
            // skill 용 다른 attack context 를 만들어야할지, 아래 context 를 추상화 할지 결정
            
            // var attackContext = AttackContextFactory.Instance.GenerateRanged(new AttackContextDto(
            //     _attacker.StatSheet.PhysicalAttack.CurrentValue,
            //     context.Attacker,
            //     context.Victim
            // ));
            //
            // attackContext.TryAttack();
            //
            // Debug.Log("불화살 발사!!");
        }

        public bool CanExecute(CanExecuteContext context)
        {
            return context.Victim;
        }
    }
}