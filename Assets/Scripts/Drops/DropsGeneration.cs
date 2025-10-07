using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsGeneration : MonoBehaviour
{
    public GameObject goldPrefab;
    public GameObject silverPrefab;
    [Range(0f, 1f)] public float dropChance = 0.7f;

    public void DropItem()
    {
        GameObject itemToDrop = (Random.value > dropChance) ? goldPrefab : silverPrefab;
        Instantiate(itemToDrop, transform.position, Quaternion.identity);
    }
}
