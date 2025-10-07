using System.Collections;
using UnityEngine;

public class Buff_Flying : BuffBase
{
    protected override void ApplyEffect()
    {
        player.CanFlying = true;
        StartCoroutine(FlyingDuration());
    }

    private IEnumerator FlyingDuration()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(duration);
        RemoveEffect();
    }

    protected override void RemoveEffect()
    {
        player.CanFlying = false;
        Destroy(gameObject);
    }
}
