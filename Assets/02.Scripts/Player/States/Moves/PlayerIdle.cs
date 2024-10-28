using UnityEngine;

public class PlayerIdle : BaseState<Player>
{
    private readonly PlayerState[] actionableStates =
    {
        PlayerState.InAir,
        PlayerState.Interacting,
        PlayerState.GravityFlipping,
        PlayerState.Running,
    };

    public override void Enter(Player player)
    {
        // 애니메이션 재생
        player.skeletonAnimation.state.SetAnimation(0, "idle", true);

        // 이동 속도 설정
        player.currentMoveDirection = Vector2.zero;
        player.currentSpeed = 0;
    }

    public override void Execute(Player player)
    {
        // 이동 가능한 상태 체크
        foreach (var state in actionableStates)
        {
            if (player.playerStates[state].CheckAction(player))
            {
                player.playerStateMachine.ChangeState(player.playerStates[state]);
                return;
            }
        }
    }

    public override bool CheckAction(Player player)
    {
        return true;
    }

    public override void Exit(Player player)
    {

    }
}
