public abstract class BaseState<T> where T : class
{
    // 상태 시작 시 실행
    public abstract void Enter(T entity);

    // 매 프레임 상태 업데이트
    public abstract void Execute(T entity);

    // 행동 가능 여부 확인
    public abstract bool CheckAction(T entity);

    // 상태 종료 시 실행
    public abstract void Exit(T entity);
}
