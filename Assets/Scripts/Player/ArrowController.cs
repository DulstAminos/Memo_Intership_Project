using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("Normal")]
    public float speed = 10f;
    public float gravity = 0.5f;
    public float wallLifeTime = 5f;
    public LayerMask wallLayer;
    public LayerMask enemyLayer;

    [Header("Blink")]
    public float blinkStartTime = 2.5f;
    private SpriteRenderer spriteRenderer;
    private bool isBlinking = false;
    private float blinkInterval;

    private Rigidbody2D rb;
    private PlatformEffector2D platformEffector;
    private bool isStuck = false;
    private float destroyTime;

    [Header("Sound")]
    private GameObject audioPlayer;
    public AudioClip stuckSound;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platformEffector = GetComponent<PlatformEffector2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioPlayer = GameObject.FindGameObjectWithTag("Audio");
        platformEffector.enabled = false;
        destroyTime = wallLifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStuck)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y - gravity * Time.deltaTime);
            if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(transform.localScale.x))
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            destroyTime -= Time.deltaTime;
            HandleBlinkingEffect();
            if (destroyTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void HandleBlinkingEffect()
    {
        float timeLeft = destroyTime;

        // 当剩余时间小于blinkStartTime时开始闪烁
        if (timeLeft <= blinkStartTime && !isBlinking)
        {
            isBlinking = true;
        }

        if (isBlinking)
        {
            // 计算闪烁频率(剩余时间越少频率越高)
            blinkInterval = Mathf.Lerp(0.2f, 0.5f, timeLeft / blinkStartTime);

            // 使用PingPong函数实现规律闪烁
            float alpha = Mathf.PingPong(Time.time / blinkInterval, 1f);
            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-1);
                Destroy(gameObject);
            }
        }
        else if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            isStuck = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            audioPlayer.GetComponent<AudioController>().PlaySound(stuckSound);

            GetComponent<Collider2D>().isTrigger = false;
            gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
            platformEffector.enabled = true;
            GetComponent<Collider2D>().usedByEffector = true;
        }
    }
}
