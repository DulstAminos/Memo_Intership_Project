using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 出生点
    [Header("BirthPosition")]
    public Transform birthPosition;

    // 生命值相关
    private int maxHealth = 3;
    private int currentHealth;
    public int MaxHealth { get { return maxHealth; } }
    public int CurrentHealth { get { return currentHealth; } }

    // 移动相关
    [Header("Move")]
    public string horizontalAxis;
    public float moveSpeed = 1.0f;
    public float moveSpeedLinitation = 10f;

    private Rigidbody2D rb;
    private float horizontal;
    private bool facingRight = true;

    // 跳跃相关
    [Header("Jump")]
    public float jumpSpeed = 1.0f;
    public Transform GroundCheck;
    public LayerMask GroundLayer;

    // 受伤后击飞相关
    [Header("Knockback")]
    public float knockbackForceX = 1f;
    public float knockbackForceY = 3f;
    public float knockbackTime = 1.0f;
    private bool isKnockback = false;
    private bool isInvincible = false;

    // 无敌相关
    [Header("Invincible")]
    public float invincibleTime = 1.0f;
    public float blinkInterval = 0.15f; // 闪烁间隔时间
    private SpriteRenderer spriteRenderer;
    private Coroutine blinkCoroutine;

    // 死亡相关
    [Header("Death")]
    public float deathKnockbackSpeedX = 0.3f;
    public float deathKnockbackSpeedY = 2f;
    public float deathKnockbackGravity = 3.0f;
    public float deathKnockbackTime = 2.5f;
    private float destroyTime;
    private bool isDead = false;

    private Collider2D col;
    private BoundaryLimitation boundaryLimitation;

    // 射击相关
    [Header("Shoot")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float shootCooldown = 0.5f;
    private float lastShotTime;

    // Buff相关（飞行Buff）
    [Header("Fly")]
    public float flyUpSpeed = 0.7f;
    private bool canFlying = false;
    public bool CanFlying { get { return canFlying; } set { canFlying = value; } }

    // 动画相关
    private Animator animator;

    // 音乐音效相关
    [Header("Sound")]
    public GameObject audioPlayer;
    public AudioClip jumpSound;
    public AudioClip shootSound;
    public AudioClip injuredSound;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = birthPosition.position;
        currentHealth = maxHealth;
        destroyTime = deathKnockbackTime;
        lastShotTime = Time.time - shootCooldown;
        rb = GetComponent<Rigidbody2D>();
        col = rb.GetComponent<Collider2D>();
        boundaryLimitation = GetComponent<BoundaryLimitation>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            horizontal = Input.GetAxisRaw(horizontalAxis); // 获取水平轴向输入
            if (!isKnockback)
            {
                // 动画水平控制
                if (horizontal != 0)
                {
                    animator.SetFloat("MoveX", 1.0f);
                }
                else
                {
                    animator.SetFloat("MoveX", 0);
                }
                // 动画竖直控制
                if (rb.velocity.y > 0.1f)
                {
                    animator.SetFloat("MoveY", 1.0f);
                }
                else if (rb.velocity.y < -0.1f)
                {
                    animator.SetFloat("MoveY", -1.0f);
                }
                else
                {
                    animator.SetFloat("MoveY", 0);
                }
                // 飞行判断
                if (canFlying)
                {
                    animator.SetBool("canFly", true);
                    Fly();
                }
                else
                {
                    animator.SetBool("canFly", false);
                    Jump();
                }
                Shoot();
            }
        }
        else
        {
            animator.SetBool("isDead", true);
            rb.velocity = new Vector2(-1 * transform.localScale.x * deathKnockbackSpeedX, rb.velocity.y - deathKnockbackGravity * Time.deltaTime);
            destroyTime -= Time.deltaTime;
            if (destroyTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && !isKnockback) Move();
        // 速度限制
        if (MathF.Abs(rb.velocity.x) > moveSpeedLinitation) rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * moveSpeedLinitation, rb.velocity.y);
        if (MathF.Abs(rb.velocity.y) > moveSpeedLinitation) rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.y) * moveSpeedLinitation);
    }

    // 控制玩家移动
    private void Move()
    {
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        // 转向
        if ((facingRight && horizontal < 0) || (!facingRight && horizontal > 0))
        {
            transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
            facingRight = !facingRight;
        }
    }

    // 控制玩家跳跃
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && OnGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            audioPlayer.GetComponent<AudioController>().PlaySound(jumpSound);
        }
    }

    // 控制玩家飞行
    private void Fly()
    {
        if ((Input.GetKey(KeyCode.W)))
        {
            rb.velocity = new Vector2(rb.velocity.x, flyUpSpeed);
        }
    }

    // 判断玩家是否在地面上
    private bool OnGround()
    {
        return Physics2D.OverlapCapsule(GroundCheck.position, new Vector2(0.25f, 0.04f), CapsuleDirection2D.Horizontal, 0, GroundLayer);
    }

    // 改变生命值
    public void ChangeHealth(int amount)
    {
        if (isInvincible && amount < 0) return;
        currentHealth = Math.Clamp(currentHealth + amount, 0, maxHealth);
        if (amount < 0 && currentHealth != 0)
        {
            audioPlayer.GetComponent<AudioController>().PlaySound(injuredSound);
            StartCoroutine(InvinciblilityCoroutine());
            // 受伤击飞效果
            StartCoroutine(KnockbackCoroutine());
            Vector2 knockbackForce = new Vector2(-1 * transform.localScale.x * knockbackForceX, knockbackForceY);
            rb.velocity = Vector2.zero; // 先清零当前速度
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }
        if (currentHealth == 0)
        {
            audioPlayer.GetComponent<AudioController>().PlaySound(injuredSound);
            isDead = true;
            col.enabled = false;
            boundaryLimitation.enabled = false;
            rb.isKinematic = true;
            rb.velocity = new Vector2(-1 * transform.localScale.x * deathKnockbackSpeedX, deathKnockbackSpeedY);
        }
        //Debug.Log(currentHealth + "/" + maxHealth);
    }

    private IEnumerator KnockbackCoroutine()
    {
        animator.SetBool("isInjured", true);
        isKnockback = true;
        yield return new WaitForSeconds(knockbackTime);
        animator.SetBool("isInjured", false);
        isKnockback = false;
    }

    public IEnumerator InvinciblilityCoroutine()
    {
        isInvincible = true;

        // 开始闪烁效果
        blinkCoroutine = StartCoroutine(BlinkEffect());

        // 获取玩家所有碰撞体
        Collider2D[] playerColliders = GetComponentsInChildren<Collider2D>();

        // 忽略与怪物的碰撞
        foreach (var col in playerColliders)
        {
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Enemy"),
                true);

            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("EnemyIgnore"),
                true);
        }

        yield return new WaitForSeconds(invincibleTime);

        // 停止闪烁效果
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            spriteRenderer.enabled = true;
        }

        // 恢复与怪物的碰撞
        foreach (var col in playerColliders)
        {
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Enemy"),
                false);

            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("EnemyIgnore"),
                false);
        }

        isInvincible = false;
    }

    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastShotTime > shootCooldown)
        {
            animator.SetTrigger("Shoot"); // 播放射击动画
            audioPlayer.GetComponent<AudioController>().PlaySound(shootSound);
            GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
            arrow.GetComponent<ArrowController>().speed = facingRight ? Mathf.Abs(arrow.GetComponent<ArrowController>().speed) :
                                                                       -Mathf.Abs(arrow.GetComponent<ArrowController>().speed);
            lastShotTime = Time.time;
        }
    }
}
