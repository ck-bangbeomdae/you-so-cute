using UnityEngine;

public class PlayerInAir : BaseState<Player>
{
    public override void Enter(Player player)
    {
        // 애니메이션 재생
        player.skeletonAnimation.state.SetAnimation(0, "flipping", false);
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

        if (player.IsGrounded)
        {
            player.playerStateMachine.ChangeState(player.playerStates[PlayerState.Idle]);
        }
    }

    public override bool CheckAction(Player player)
    {
        return !player.IsGrounded;
    }

    public override void Exit(Player player)
    {

    }
}
