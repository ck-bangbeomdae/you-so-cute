public class PlayerInteracting : BaseState<Player>
{
    public override void Enter(Player player)
    {
        // TODO : 상호작용 가능한 오브젝트에 따른 애니메이션 재생

        player.closestInteractable.Interact(player);
    }

    public override void Execute(Player player)
    {
        // TODO : 상호작용 가능한 오브젝트에 따른 Idle 상태 전환 조건 구현

        // Idle 상태 전환
        player.playerStateMachine.ChangeState(player.playerStates[PlayerState.Idle]);
    }

    public override bool CheckAction(Player player)
    {
        return player.closestInteractable != null && InputManager.InteractAction.WasPressedThisFrame();
    }

    public override void Exit(Player player)
    {

    }
}
