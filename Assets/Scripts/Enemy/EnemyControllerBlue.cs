using UnityEngine;

public class EnemyControllerBlue : MonoBehaviour
{
    [Header("Damage")]
    public int damageAmount = 1;
    [Header("Move")]
    public float moveSpeed = 1f;
    //private int health = 1;
    //public int Health { get => health; set => health = value; }

    public LayerMask obstacleLayer;
    public Transform frontCheck;

    private bool movingRight = true;
    private float lastTrunTime;
    public float turnCooldown = 0.5f;

    private Rigidbody2D rb;
    private EnemyHealth health;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        lastTrunTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!health.IsDead)
        {
            float direction = movingRight ? 1 : -1;
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

            bool hitObstacle = Physics2D.OverlapCapsule(frontCheck.position, new Vector2(0.04f, 0.46f), CapsuleDirection2D.Vertical, 0, obstacleLayer);

            if (hitObstacle)
            {
                TurnAround();
            }
        }
    }

    void TurnAround()
    {
        if (Time.time - lastTrunTime < turnCooldown) return;
        movingRight = !movingRight;
        transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
        lastTrunTime = Time.time;
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
