using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ������
    [Header("BirthPosition")]
    public Transform birthPosition;

    // ����ֵ���
    private int maxHealth = 3;
    private int currentHealth;
    public int MaxHealth { get { return maxHealth; } }
    public int CurrentHealth { get { return currentHealth; } }

    // �ƶ����
    [Header("Move")]
    public string horizontalAxis;
    public float moveSpeed = 1.0f;
    public float moveSpeedLinitation = 10f;

    private Rigidbody2D rb;
    private float horizontal;
    private bool facingRight = true;

    // ��Ծ���
    [Header("Jump")]
    public float jumpSpeed = 1.0f;
    public Transform GroundCheck;
    public LayerMask GroundLayer;

    // ���˺�������
    [Header("Knockback")]
    public float knockbackForceX = 1f;
    public float knockbackForceY = 3f;
    public float knockbackTime = 1.0f;
    private bool isKnockback = false;
    private bool isInvincible = false;

    // �޵����
    [Header("Invincible")]
    public float invincibleTime = 1.0f;
    public float blinkInterval = 0.15f; // ��˸���ʱ��
    private SpriteRenderer spriteRenderer;
    private Coroutine blinkCoroutine;

    // �������
    [Header("Death")]
    public float deathKnockbackSpeedX = 0.3f;
    public float deathKnockbackSpeedY = 2f;
    public float deathKnockbackGravity = 3.0f;
    public float deathKnockbackTime = 2.5f;
    private float destroyTime;
    private bool isDead = false;

    private Collider2D col;
    private BoundaryLimitation boundaryLimitation;

    // ������
    [Header("Shoot")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float shootCooldown = 0.5f;
    private float lastShotTime;

    // Buff��أ�����Buff��
    [Header("Fly")]
    public float flyUpSpeed = 0.7f;
    private bool canFlying = false;
    public bool CanFlying { get { return canFlying; } set { canFlying = value; } }

    // �������
    private Animator animator;

    // ������Ч���
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
            horizontal = Input.GetAxisRaw(horizontalAxis); // ��ȡˮƽ��������
            if (!isKnockback)
            {
                // ����ˮƽ����
                if (horizontal != 0)
                {
                    animator.SetFloat("MoveX", 1.0f);
                }
                else
                {
                    animator.SetFloat("MoveX", 0);
                }
                // ������ֱ����
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
                // �����ж�
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
        // �ٶ�����
        if (MathF.Abs(rb.velocity.x) > moveSpeedLinitation) rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * moveSpeedLinitation, rb.velocity.y);
        if (MathF.Abs(rb.velocity.y) > moveSpeedLinitation) rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.y) * moveSpeedLinitation);
    }

    // ��������ƶ�
    private void Move()
    {
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        // ת��
        if ((facingRight && horizontal < 0) || (!facingRight && horizontal > 0))
        {
            transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
            facingRight = !facingRight;
        }
    }

    // ���������Ծ
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && OnGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            audioPlayer.GetComponent<AudioController>().PlaySound(jumpSound);
        }
    }

    // ������ҷ���
    private void Fly()
    {
        if ((Input.GetKey(KeyCode.W)))
        {
            rb.velocity = new Vector2(rb.velocity.x, flyUpSpeed);
        }
    }

    // �ж�����Ƿ��ڵ�����
    private bool OnGround()
    {
        return Physics2D.OverlapCapsule(GroundCheck.position, new Vector2(0.25f, 0.04f), CapsuleDirection2D.Horizontal, 0, GroundLayer);
    }

    // �ı�����ֵ
    public void ChangeHealth(int amount)
    {
        if (isInvincible && amount < 0) return;
        currentHealth = Math.Clamp(currentHealth + amount, 0, maxHealth);
        if (amount < 0 && currentHealth != 0)
        {
            audioPlayer.GetComponent<AudioController>().PlaySound(injuredSound);
            StartCoroutine(InvinciblilityCoroutine());
            // ���˻���Ч��
            StartCoroutine(KnockbackCoroutine());
            Vector2 knockbackForce = new Vector2(-1 * transform.localScale.x * knockbackForceX, knockbackForceY);
            rb.velocity = Vector2.zero; // �����㵱ǰ�ٶ�
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

        // ��ʼ��˸Ч��
        blinkCoroutine = StartCoroutine(BlinkEffect());

        // ��ȡ���������ײ��
        Collider2D[] playerColliders = GetComponentsInChildren<Collider2D>();

        // ������������ײ
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

        // ֹͣ��˸Ч��
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            spriteRenderer.enabled = true;
        }

        // �ָ���������ײ
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
            animator.SetTrigger("Shoot"); // �����������
            audioPlayer.GetComponent<AudioController>().PlaySound(shootSound);
            GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
            arrow.GetComponent<ArrowController>().speed = facingRight ? Mathf.Abs(arrow.GetComponent<ArrowController>().speed) :
                                                                       -Mathf.Abs(arrow.GetComponent<ArrowController>().speed);
            lastShotTime = Time.time;
        }
    }
}
