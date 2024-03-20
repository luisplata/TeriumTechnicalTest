using SL;
using UnityEngine;

public class RocketLauncher : WeaponBase
{
    [SerializeField] private float timeToStun;
    public override void ApplyDamageEffect(Player player)
    {
        //ServiceLocator.Instance.GetService<IDebug>().Log("RocketLauncher ApplyDamageEffect");
        player.Stun(timeToStun);
    }
}