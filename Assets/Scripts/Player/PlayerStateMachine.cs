

public class PlayerStateMachine 
{
    private readonly PlayerController player;
    public IPlayerState CurrentState {  get; private set; }

    public PlayerStateMachine(PlayerController player)
    {
        this.player = player;
    }
    public void Initialize(IPlayerState startingState)
    {
        if(startingState == null)
        {
            return;
        }
        CurrentState = startingState;
        CurrentState.EnterState(player);
    }
    public void ChangeState(IPlayerState newState)
    {
        if (newState == null || newState == CurrentState) {
            return;

        }
        CurrentState?.ExitState(player);
        CurrentState = newState;
        CurrentState.EnterState(player);


    }
   

    // Update is called once per frame
    public void Update()
    {
        CurrentState?.UpdateState(player);
    }
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdateState(player);
    }
}
