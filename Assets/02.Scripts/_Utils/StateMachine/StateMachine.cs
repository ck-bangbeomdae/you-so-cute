public class StateMachine<T> where T : class
{
    public T OwnerEntity;
    public BaseState<T> CurrentState;
    public BaseState<T> PreviousState;

    public void Setup(T owner, BaseState<T> entryState)
    {
        OwnerEntity = owner;
        CurrentState = null;
        PreviousState = null;

        ChangeState(entryState);
    }

    public void Execute()
    {
        if (CurrentState != null)
        {
            CurrentState.Execute(OwnerEntity);
        }
    }

    public void ChangeState(BaseState<T> newState)
    {
        if (newState == null)
        {
            return;
        }

        if (CurrentState != null)
        {
            PreviousState = CurrentState;
            CurrentState.Exit(OwnerEntity);
        }

        CurrentState = newState;
        CurrentState.Enter(OwnerEntity);
    }

    public void RevertToPreviousState()
    {
        ChangeState(PreviousState);
    }
}
