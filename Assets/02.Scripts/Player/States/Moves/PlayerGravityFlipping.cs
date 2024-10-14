using UnityEngine;

public class PlayerGravityFlipping : BaseState<Player>
{
    public override void Enter(Player player)
    {
        // 애니메이션 재생
        player.skeletonAnimation.state.SetAnimation(0, "gravity_flipping", false);

        // 중력 반전
        player.GravityFlip();
    }

    public override void Execute(Player player)
    {
        if (InputManager.MoveAction.IsPressed())
        {
            player.currentMoveDirection = InputManager.MoveAction.ReadValue<Vector2>();
            player.currentSpeed = player.moveSpeed;
        }
        else
        {
            player.currentMoveDirection = Vector2.zero;
            player.currentSpeed = 0;
        }

        if (player.isGrounded)
        {
            player.playerStateMachine.ChangeState(player.playerStates[PlayerState.Idle]);
        }
    }

    public override bool CheckAction(Player player)
    {
        return InputManager.GravityFlipAction.WasPressedThisFrame() && player.isGrounded;
    }

    public override void Exit(Player player)
    {

    }
}
