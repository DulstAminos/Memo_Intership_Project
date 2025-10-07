using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    public GameObject player;
    public Image Health;
    public Text Score;

    public Sprite[] HealthSprite;

    private int playerHealth;
    private int playerScore;

    // Start is called before the first frame update
    void Start()
    {
        Health.sprite = HealthSprite[3];
        Score.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = player.GetComponent<PlayerController>().CurrentHealth;
        playerScore = player.GetComponent<Score>().GetScore();
        Health.sprite = HealthSprite[playerHealth];
        Score.text = playerScore.ToString();
    }
}
