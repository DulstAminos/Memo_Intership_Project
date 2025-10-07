using System.Collections;
using UnityEngine;

public class Buff_Invincible : BuffBase
{
    private float originalInvincibleTime;
    protected override void ApplyEffect()
    {
        originalInvincibleTime = player.invincibleTime;
        player.invincibleTime = duration;
        player.StartCoroutine(player.InvinciblilityCoroutine());
        StartCoroutine(InvincibleDuration());
    }

    private IEnumerator InvincibleDuration()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(duration);
        RemoveEffect();
    }

    protected override void RemoveEffect()
    {
        player.invincibleTime = originalInvincibleTime;
        Destroy(gameObject);
    }

}
