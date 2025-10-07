using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUI : MonoBehaviour
{
    public void playGame()
    {
        SceneManager.LoadScene("GameScene1");
    }
}
