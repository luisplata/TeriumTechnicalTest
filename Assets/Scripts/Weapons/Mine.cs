using SL;
using UnityEngine;

public class Mine : WeaponBase
{
    [SerializeField] private float timeToBlind;
    public override void ApplyDamageEffect(Player player)
    {
        //ServiceLocator.Instance.GetService<IDebug>().Log("Mine ApplyDamageEffect");
        player.Blind(timeToBlind);
    }
}