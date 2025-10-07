using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DisappearPlatform : MonoBehaviour
{
    public float disappearDelay = 1.0f;
    public float reappearDelay = 2.0f;

    public float blinkStartTime = 0.5f;
    //private bool isBlinking = false;
    private float blinkTimer;

    private Collider2D tilemapCollider;
    private TilemapRenderer tilemapRenderer;
    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        tilemapCollider = GetComponent<Collider2D>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        originalPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DisappearSequence());
        }
    }

    private IEnumerator DisappearSequence()
    {
        //yield return new WaitForSeconds(disappearDelay);
        // ��˸�׶�
        blinkTimer = disappearDelay;
        while (blinkTimer > 0)
        {
            blinkTimer -= Time.deltaTime;

            // ��ʣ��ʱ��С��blinkStartTimeʱ��ʼ��˸
            if (blinkTimer <= blinkStartTime)
            {
                float blinkSpeed = Mathf.Lerp(0.1f, 0.2f, 1 - (blinkTimer / blinkStartTime));
                tilemapRenderer.enabled = Mathf.PingPong(Time.time / blinkSpeed, 1) > 0.5f;
            }
            yield return null;
        }

        // ��ʧ�׶�
        tilemapCollider.enabled = false;
        tilemapRenderer.enabled = false;

        yield return new WaitForSeconds(reappearDelay);

        // ���ֽ׶�
        tilemapCollider.enabled = true;
        tilemapRenderer.enabled = true;
        transform.position = originalPosition;
    }
}
