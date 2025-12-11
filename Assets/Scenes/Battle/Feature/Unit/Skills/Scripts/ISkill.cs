using Scenes.Battle.Feature.Unit.Skills.Contexts;

namespace Scenes.Battle.Feature.Unit.Skills
{
    public interface ISkill
    {
        /// <summary>
        /// 유닛이 초기화되어 스킬 정보가 유닛에 기입 될 떄
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(InitializeContext context);
        
        /// <summary>
        /// 스킬 실행 시
        /// </summary>
        /// <param name="context"></param>
        public void Execute(ExecuteContext context);
        
        /// <summary>
        /// 스킬 실행 가능 여부 판단
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool CanExecute(CanExecuteContext context);
    }
}
