using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public GameObject audioPlayer;
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        audioPlayer.GetComponent<AudioController>().PalyMusic(audioClip);
    }
}
