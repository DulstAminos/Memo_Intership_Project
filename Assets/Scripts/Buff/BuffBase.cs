using UnityEngine;

public abstract class BuffBase : MonoBehaviour
{
    public BuffType buffType;
    public float duration = 5f;
    public float existTime = 10f;

    protected PlayerController player;
    private float spawnTime;
    private bool isUsed = false;

    [Header("Sound")]
    private GameObject audioPlayer;
    public AudioClip getSound;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.time;
        audioPlayer = GameObject.FindGameObjectWithTag("Audio");
    }

    // Update is called once per frame
    void Update()
    {
        // Buff存在时间到期自动消失
        if (!isUsed && Time.time - spawnTime > existTime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isUsed = true;
            audioPlayer.GetComponent<AudioController>().PlaySound(getSound);
            player = other.GetComponent<PlayerController>();
            ApplyEffect();
        }
    }

    protected abstract void ApplyEffect();

    protected abstract void RemoveEffect();
}
