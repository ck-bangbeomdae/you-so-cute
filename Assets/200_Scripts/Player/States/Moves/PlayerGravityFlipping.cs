public class PlayerGravityFlipping : BaseState<Player>
{
    public override void Enter(Player player)
    {
        // 중력 반전 효과음 재생
        var sfxSoundInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player");
        sfxSoundInstance.setParameterByNameWithLabel("Player", "Flip");
        sfxSoundInstance.start();
        sfxSoundInstance.release();

        // 중력 반전
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
