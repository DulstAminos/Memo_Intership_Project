using UnityEngine;

public class Score : MonoBehaviour
{
    private int score;

    [Header("Sound")]
    public GameObject audioPlayer;
    public AudioClip goldSound;
    public AudioClip silverSound;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            if (collision.gameObject.name.Contains("Gold"))
            {
                score += 100;
                audioPlayer.GetComponent<AudioController>().PlaySound(goldSound);
            }
            else if (collision.gameObject.name.Contains("Silver"))
            {
                score += 50;
                audioPlayer.GetComponent<AudioController>().PlaySound(silverSound);
            }
            Destroy(collision.gameObject);
            //Debug.Log("Scores:" + score);
        }
    }

    public int GetScore() { return score; }
}
