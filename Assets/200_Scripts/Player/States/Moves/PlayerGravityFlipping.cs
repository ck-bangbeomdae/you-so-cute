using UnityEngine;

public class PlayerGravityFlipping : BaseState<Player>
{
    public override void Enter(Player player)
    {
        // 중력 반전
        player.GravityFlip();
    }

    public override void Execute(Player player)
    {
        // 중력 반전시 먼지 떨어지는 파티클 재생
        if (player.r_dustParticlePrefab != null && player.s_dustParticlePrefab != null)
        {
            if (player.IsGravityFlipped)
            {
                player.s_dustParticlePrefab.GetComponent<ParticleSystem>().Stop();
                player.r_dustParticlePrefab.GetComponent<ParticleSystem>().Play();

                foreach (GameObject p in player.r_flipDustParticlePrefab)
                {
                    p.GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                player.r_dustParticlePrefab.GetComponent<ParticleSystem>().Stop();
                player.s_dustParticlePrefab.GetComponent<ParticleSystem>().Play();

                foreach (GameObject p in player.s_flipDustParticlePrefab)
                {
                    p.GetComponent<ParticleSystem>().Play();
                }
            }
        }

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
