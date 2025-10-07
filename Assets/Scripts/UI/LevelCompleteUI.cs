using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    public static LevelCompleteUI instance;

    public int levelNum = 3;
    public GameObject levelCompleteMenuUI;
    public Text levelScoreText;
    public Text totalScoreText;

    public float checkInterval = 2f;
    private int totalScore = 0;
    private float lastCheckTime;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            totalScore = instance.totalScore;
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkInterval)
        {
            lastCheckTime = Time.time;
            CheckEnemies();

            if (levelCompleteMenuUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
            {
                ContinueToNextLevel();
            }
        }
    }

    void CheckEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0 && !levelCompleteMenuUI.activeSelf)
        {
            ShowLevelComplete();
        }
    }

    void ShowLevelComplete()
    {
        // ��ȡ��ǰ�ؿ�����
        int currentScore = FindObjectOfType<Score>().GetScore();
        totalScore += currentScore;

        // ����UI
        levelScoreText.text = currentScore.ToString();
        totalScoreText.text = totalScore.ToString();

        // ��ʾ����
        levelCompleteMenuUI.SetActive(true);
        Time.timeScale = 0f; // ��ͣ��Ϸ
    }

    public void ContinueToNextLevel()
    {
        Time.timeScale = 1f;
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index < levelNum)
        {
            SceneManager.LoadScene(index + 1);
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }
}
