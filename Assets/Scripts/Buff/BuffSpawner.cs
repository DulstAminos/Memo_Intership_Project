using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    public GameObject[] buffPrefabs;
    public float spawnInterval = 15f;
    public Transform[] spawnPoints;

    private float lastSpawnTime;
    // Start is called before the first frame update
    void Start()
    {
        lastSpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawnTime > spawnInterval)
        {
            SpawnRandomBuff();
            lastSpawnTime = Time.time;
        }
    }

    void SpawnRandomBuff()
    {
        int randomPoint = Random.Range(0, spawnPoints.Length);
        int randomBuff = Random.Range(0, buffPrefabs.Length);

        // 清除该位置现有Buff
        foreach (Transform child in spawnPoints[randomPoint])
        {
            Destroy(child.gameObject);
        }

        Instantiate(buffPrefabs[randomBuff],
                  spawnPoints[randomPoint].position,
                  Quaternion.identity,
                  spawnPoints[randomPoint]);
    }
}
