using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Collider2D playerCollider;
    private Rigidbody2D rigidbody;
    public Vector2 MoveInput { get; private set; }
    private PlayerStateMachine stateMachine;
    public bool IsDead { get; private set; }
    private static readonly int SpeedHash =
       Animator.StringToHash("Speed");

    private static readonly int DieHash =
        Animator.StringToHash("Die");
    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerDieState DieState { get; private set; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (playerCollider == null) 
        {
            playerCollider = GetComponent<Collider2D>();

        }
        CreateStates();
    }
    void Start()
    {
        stateMachine.Initialize(IdleState);
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        stateMachine.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
    private void OnDisable()
    {
        MoveInput = Vector2.zero;
        StopMovement();
    }
    private void CreateStates()
    {
        stateMachine = new PlayerStateMachine(this);
        IdleState = new PlayerIdleState();
        RunState = new PlayerRunState();
        DieState = new PlayerDieState();
    }
    private void ReadInput()
    {
        if (IsDead)
        {
            MoveInput = Vector2.zero;
            return;
        }
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        MoveInput = new Vector2(inputX, inputY).normalized;
    }
    public void ChangeState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }
    public void ApplyMovement()
    {
        if (IsDead)
        {
            StopMovement();
            return;
        }

        rigidbody.linearVelocity = MoveInput * moveSpeed;
    }
    public void StopMovement()
    {
        if(rigidbody != null)
        {
            rigidbody.linearVelocity = Vector2.zero;
        }
    }
    public void UpdateFacingDirection()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (MoveInput.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (MoveInput.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }
    public void SetAnimationSpeed(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat(SpeedHash, speed);
        }
    }

    public void PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.ResetTrigger(DieHash);
            animator.SetTrigger(DieHash);
        }
    }

    public void DisableCollision()
    {
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }
    }

    public void EnableCollision()
    {
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }
    }

    public void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        MoveInput = Vector2.zero;

        ChangeState(DieState);
    }
}
