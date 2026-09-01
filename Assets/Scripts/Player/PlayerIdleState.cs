
public class PlayerIdleState : IPlayerState
{
    public void EnterState(PlayerController player)
    {
        player.StopMovement();
        player.SetAnimationSpeed(0f);
    }
    public void UpdateState(PlayerController player)
    {
        if (player.IsDead)
        {
            player.ChangeState(player.DieState);
            return;
        }
        if (player.MoveInput.sqrMagnitude > 0.01f)
        {
            player.ChangeState(player.RunState);
        }

    }
    public void FixedUpdateState(PlayerController player)
    {
        player.StopMovement();
    }
    public void ExitState(PlayerController player)
    {

    }
}
