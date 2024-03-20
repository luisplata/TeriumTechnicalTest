using SL;
using UnityEngine;

public class GoldenEye : WeaponBase
{
    [SerializeField] private float timeToKill;
    public override void ApplyDamageEffect(Player player)
    {
        ServiceLocator.Instance.GetService<IDebug>().Log("GoldenEye ApplyDamageEffect");
        player.Kill(timeToKill);
    }
}