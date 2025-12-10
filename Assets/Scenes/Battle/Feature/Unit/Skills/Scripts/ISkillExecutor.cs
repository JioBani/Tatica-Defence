using Scenes.Battle.Feature.Unit.Skills.Contexts;

namespace Scenes.Battle.Feature.Unit.Skills
{
    public interface ISkillExecutor
    {
        /// <summary>
        /// 유닛이 초기화되어 스킬 정보가 유닛에 기입 될 떄
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(SkillInitializeContext context);
        
        /// <summary>
        /// 스킬 실행 시
        /// </summary>
        /// <param name="context"></param>
        public void Execute(SkillRuntimeContext context);
    }
}
