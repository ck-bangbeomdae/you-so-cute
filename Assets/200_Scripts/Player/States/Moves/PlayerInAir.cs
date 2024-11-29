using UnityEngine;

public class PlayerInAir : BaseState<Player>
{
    public override void Enter(Player player)
    {
        // 애니메이션 재생
        if (!player.isCollidingWithJumpPad && !player.isTriggerGravityFlipKeyboard)
        {
            player.skeletonAnimation.state.SetAnimation(0, "flipping_up", false);
        }
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

        if (player.isTriggerGravityFlipKeyboard)
        {
            player.spineAnimationObject.transform.Rotate(new Vector3(0f, 0f, player.spineAnimationObject.transform.rotation.z + 1.5f));
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
