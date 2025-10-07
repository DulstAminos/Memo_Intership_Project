public class Buff_Recovery : BuffBase
{
    protected override void ApplyEffect()
    {
        player.ChangeHealth(1);
        Destroy(gameObject);
    }

    protected override void RemoveEffect()
    {
        // 恢复效果无需移除
    }
}
