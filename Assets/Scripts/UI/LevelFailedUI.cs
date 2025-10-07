using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFailedUI : MonoBehaviour
{
    public static LevelFailedUI instance;
    public GameObject gameOverUI;
    public Text finalScoreText;

    private Score score;
    private int currentScore;
    private int totalScore = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            if (score = FindObjectOfType<Score>())
            {
                currentScore = score.GetScore();
            }
            CheckPlayer();
        }
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartLevel();
        }
    }

    void CheckPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0 && !gameOverUI.activeSelf)
        {
            PlayerDied();
        }
    }

    public void PlayerDied()
    {
        totalScore += currentScore;
        finalScoreText.text = totalScore.ToString();
        gameOverUI.SetActive(true);
        isGameOver = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
