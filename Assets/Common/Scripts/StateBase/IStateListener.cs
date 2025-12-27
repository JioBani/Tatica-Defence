using System;

namespace Common.Scripts.StateBase
{
    /// <summary>
    /// StateBase의 생명주기 이벤트를 감지하는 인터페이스
    /// 하나의 클래스가 여러 StateBaseController를 리스닝할 수 있도록 제네릭 인터페이스 구현
    /// </summary>
    public interface IStateListener<T> where T : struct, Enum
    {
        /// <summary>
        /// 상태 진입 시 호출됩니다.
        /// </summary>
        void OnStateEnter(T stateType);

        /// <summary>
        /// 상태 실행 중 매 프레임 호출됩니다.
        /// </summary>
        void OnStateRun(T stateType);

        /// <summary>
        /// 상태 종료 시 호출됩니다.
        /// </summary>
        void OnStateExit(T stateType);
    }
}
