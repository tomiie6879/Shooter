

public class PlayerRunState : IPlayerState
{
    public void EnterState(PlayerController player)
    {
        player.SetAnimationSpeed(1f);

    }
    public void UpdateState(PlayerController player)
    {
        if (player.IsDead)
        {
            player.ChangeState(player.DieState); return;
        }
        if(player.MoveInput.sqrMagnitude <= 0.01f)
        {
            player.ChangeState(player.IdleState);
            return;
        }
        player.UpdateFacingDirection();

    }
    public void FixedUpdateState(PlayerController player)
    {
        player.ApplyMovement();
    }

    public void ExitState(PlayerController player)
    {
        player.StopMovement();
    }
}
