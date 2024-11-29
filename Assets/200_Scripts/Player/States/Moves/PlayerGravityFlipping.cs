public class PlayerGravityFlipping : BaseState<Player>
{
    public override void Enter(Player player)
    {
        // 중력 반전
        player.isTriggerGravityFlipKeyboard = true;
        player.skeletonAnimation.state.SetAnimation(0, "flipping_jump", false);
        player.GravityFlip();
    }

    public override void Execute(Player player)
    {
        player.playerStateMachine.ChangeState(player.playerStates[PlayerState.InAir]);
    }

    public override bool CheckAction(Player player)
    {
        return InputManager.GravityFlipAction.WasPressedThisFrame() && player.IsGrounded;
    }

    public override void Exit(Player player)
    {

    }
}
