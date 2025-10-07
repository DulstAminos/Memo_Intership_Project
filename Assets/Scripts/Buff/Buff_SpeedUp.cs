using System.Collections;
using UnityEngine;

public class Buff_SpeedUp : BuffBase
{
    private float originalSpeed;
    public float speedMultiplier = 1.5f;

    protected override void ApplyEffect()
    {
        originalSpeed = player.moveSpeed;
        player.moveSpeed *= speedMultiplier;
        StartCoroutine(SpeedUpDuration());
    }

    private IEnumerator SpeedUpDuration()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(duration);
        RemoveEffect();
    }

    protected override void RemoveEffect()
    {
        player.moveSpeed = originalSpeed;
        Destroy(gameObject);
    }
}
