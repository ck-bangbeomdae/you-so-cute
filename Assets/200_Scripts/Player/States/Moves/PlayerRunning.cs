using UnityEngine;

public class PlayerRunning : BaseState<Player>
{
    public override void Enter(Player player)
    {
        // 애니메이션 재생
        player.skeletonAnimation.state.SetAnimation(0, "run", true);

        // 이동 속도 설정
        player.currentSpeed = player.moveSpeed;
    }

    public override void Execute(Player player)
    {
        // InAir 상태 전환
        if (player.playerStates[PlayerState.InAir].CheckAction(player))
        {
            player.playerStateMachine.ChangeState(player.playerStates[PlayerState.InAir]);
            return;
        }

        // Flipping 상태 전환
        if (player.playerStates[PlayerState.GravityFlipping].CheckAction(player))
        {
            player.playerStateMachine.ChangeState(player.playerStates[PlayerState.GravityFlipping]);
            return;
        }

        // Running 상태 유지
        if (CheckAction(player))
        {
            player.currentMoveDirection = InputManager.MoveAction.ReadValue<Vector2>();

            // TODO : 뛰는 효과음 재생

            // TODO : 뛰는 이펙트 재생

            return;
        }

        // Idle 상태 전환
        player.playerStateMachine.ChangeState(player.playerStates[PlayerState.Idle]);
    }

    public override bool CheckAction(Player player)
    {
        return InputManager.MoveAction.IsPressed();
    }

    public override void Exit(Player player)
    {

    }
}
