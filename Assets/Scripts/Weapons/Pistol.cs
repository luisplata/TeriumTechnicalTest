using SL;
using UnityEngine;

public class Pistol : WeaponBase
{
    public override void ApplyDamageEffect(Player player)
    {
        ServiceLocator.Instance.GetService<IDebug>().Log("Pistol ApplyDamageEffect");
    }
}