public class Buff_Recovery : BuffBase
{
    protected override void ApplyEffect()
    {
        player.ChangeHealth(1);
        Destroy(gameObject);
    }

    protected override void RemoveEffect()
    {
        // �ָ�Ч�������Ƴ�
    }
}
