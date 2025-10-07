using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 1;
    private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } }

    [Header("Death")]
    public float knockbackSpeedX = 0.3f;
    public float knockbackSpeedY = 2f;
    public float knockbackGravity = 3.0f;
    public float knockbackTime = 2.5f;
    private float destroyTime;
    private bool isDead = false;
    public bool IsDead { get { return isDead; } }

    [Header("Sound")]
    private GameObject audioPlayer;
    public AudioClip deadSound;

    private Collider2D col;
    private Rigidbody2D rb;
    private BoundaryLimitation boundaryLimitation;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        destroyTime = knockbackTime;
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        boundaryLimitation = GetComponent<BoundaryLimitation>();
        animator = GetComponent<Animator>();
        audioPlayer = GameObject.FindGameObjectWithTag("Audio");
    }

    private void Update()
    {
        if (isDead)
        {
            rb.velocity = new Vector2(-1 * transform.localScale.x * knockbackSpeedX, rb.velocity.y - knockbackGravity * Time.deltaTime);
            destroyTime -= Time.deltaTime;
            if (destroyTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Math.Clamp(currentHealth + amount, 0, maxHealth);
        if (currentHealth == 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            audioPlayer.GetComponent<AudioController>().PlaySound(deadSound);
            col.enabled = false;
            boundaryLimitation.enabled = false;
            rb.isKinematic = true;
            rb.velocity = new Vector2(-1 * Mathf.Sign(rb.velocity.x) * knockbackSpeedX, knockbackSpeedY);
            GetComponent<DropsGeneration>().DropItem();
        }
    }
}
