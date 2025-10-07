using UnityEngine;

public class EnemyControllerBlack : MonoBehaviour
{
    [Header("Damage")]
    public int damageAmount = 1;
    [Header("Move")]
    public float moveSpeed = 1f;
    //private int health = 1;
    //public int Health { get => health; set => health = value; }


    public LayerMask groundLayer;
    public Transform groundCheck;
    //public LayerMask frontGroundLayer;
    //public Transform frontGroundCheck;
    public LayerMask obstacleLayer;
    public Transform frontCheck;

    private bool movingRight = false;
    private float lastTurnTime;
    public float turnCooldown = 0.5f;
    [Header("Jump")]
    public LayerMask upperPlatformLayer;
    public float jumpForce = 5f;
    public float jumpCooldown = 1f;
    private float lastJumpTime;

    private Rigidbody2D rb;
    private bool isGrounded;
    private EnemyHealth health;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        lastTurnTime = Time.time;
        lastJumpTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!health.IsDead)
        {
            float direction = movingRight ? 1 : -1;
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

            isGrounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.2f, 0.04f), CapsuleDirection2D.Horizontal, 0, groundLayer);

            if (Mathf.Abs(rb.velocity.y) > 0.1f)
            {
                animator.SetBool("isJump", true);
            }
            else if (isGrounded)
            {
                animator.SetBool("isJump", false);
            }

            bool hitObstacle = Physics2D.OverlapCapsule(frontCheck.position, new Vector2(0.04f, 0.275f), CapsuleDirection2D.Vertical, 0, obstacleLayer);
            //bool noGroundAhead = !Physics2D.OverlapCircle(frontGroundCheck.position, 0.04f, frontGroundLayer);

            if (isGrounded && (hitObstacle /*|| noGroundAhead*/))
            {
                TurnAround();
            }

            if (Time.time - lastJumpTime > jumpCooldown && isGrounded)
            {
                CheckForJump();
            }
        }
    }

    void CheckForJump()
    {
        float maxReachableHeight = CalculateMaxJumpHeight() - 0.8f;

        Vector2 checkOrigin = frontCheck.position + (movingRight ? Vector3.right * 0.4f : Vector3.left * 0.4f);
        RaycastHit2D hit = Physics2D.Raycast(checkOrigin, Vector2.up, maxReachableHeight, upperPlatformLayer);

        if (hit.collider != null)
        {
            Jump();
            lastJumpTime = Time.time;
        }
    }

    private float CalculateMaxJumpHeight()
    {
        return (jumpForce * jumpForce) / (2 * Mathf.Abs(Physics2D.gravity.y * rb.gravityScale));
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void TurnAround()
    {
        if (Time.time - lastTurnTime < turnCooldown) return;
        movingRight = !movingRight;
        transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
        lastTurnTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ChangeHealth(-damageAmount);
            }
        }
    }
}
