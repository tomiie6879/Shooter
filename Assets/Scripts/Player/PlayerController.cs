using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;

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
    private static readonly int MoveXHash =
    Animator.StringToHash("MoveX");

    private static readonly int MoveYHash =
        Animator.StringToHash("MoveY");

    private static readonly int LastMoveXHash =
        Animator.StringToHash("LastMoveX");

    private static readonly int LastMoveYHash =
        Animator.StringToHash("LastMoveY");
    private Vector2 lastMoveDirection = Vector2.down;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }

        CreateStates();
    }
    private void UpdateAnimationDirection()
    {
        if (animator == null)
        {
            return;
        }

        if (MoveInput.sqrMagnitude <= 0.01f)
        {
            animator.SetFloat(MoveXHash, 0f);
            animator.SetFloat(MoveYHash, 0f);
            return;
        }

        Vector2 animationDirection =
            GetCardinalDirection(MoveInput);

        animator.SetFloat(
            MoveXHash,
            animationDirection.x
        );

        animator.SetFloat(
            MoveYHash,
            animationDirection.y
        );

        lastMoveDirection = animationDirection;

        animator.SetFloat(
            LastMoveXHash,
            lastMoveDirection.x
        );

        animator.SetFloat(
            LastMoveYHash,
            lastMoveDirection.y
        );
    }
    private static Vector2 GetCardinalDirection(
    Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector2(
                Mathf.Sign(input.x),
                0f
            );
        }

        return new Vector2(
            0f,
            Mathf.Sign(input.y)
        );
    }
    private void Start()
    {
        lastMoveDirection = Vector2.down;

        animator.SetFloat(LastMoveXHash, 0f);
        animator.SetFloat(LastMoveYHash, -1f);
        animator.SetFloat(MoveXHash, 0f);
        animator.SetFloat(MoveYHash, 0f);

        stateMachine.Initialize(IdleState);
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        UpdateAnimationDirection();
        UpdateFacingDirection();
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

        Vector2 rawInput = new Vector2(inputX, inputY);

        const float deadZone = 0.2f;

        if (rawInput.sqrMagnitude < deadZone * deadZone)
        {
            MoveInput = Vector2.zero;
            return;
        }

        MoveInput = rawInput.normalized;
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
        if (visualRoot == null ||
            MoveInput.sqrMagnitude <= 0.01f)
        {
            return;
        }

        Vector2 direction =
            GetCardinalDirection(MoveInput);

        Vector3 scale = visualRoot.localScale;

        if (direction.x < 0f)
        {
            // Sprite gốc là West.
            scale.x = Mathf.Abs(scale.x);
        }
        else if (direction.x > 0f)
        {
            // East dùng sprite West lật ngang.
            scale.x = -Mathf.Abs(scale.x);
        }
        else
        {
            // North/South không lật.
            scale.x = Mathf.Abs(scale.x);
        }

        visualRoot.localScale = scale;
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
