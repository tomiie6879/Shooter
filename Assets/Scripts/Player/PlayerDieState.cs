public sealed class PlayerDieState : IPlayerState
{
    public void EnterState(PlayerController player)
    {
        player.StopMovement();
        player.PlayDeathAnimation();
        player.DisableCollision();
    }

    public void UpdateState(PlayerController player)
    {
        
    }

    public void FixedUpdateState(PlayerController player)
    {
        player.StopMovement();
    }

    public void ExitState(PlayerController player)
    {
       
    }
}