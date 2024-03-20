using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviourPunCallbacks
{
    [SerializeField] protected string id;
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown;
    protected GameObject weapon;
    protected bool canShoot = true;

    private void Start()
    {
        weapon = Instantiate(prefab, transform);
        canShoot = true;
    }

    public string Id => id;

    public float GetDamage()
    {
        return damage;
    }

    public bool CanShoot()
    {
        return canShoot;
    }

    public void Shoot()
    {
        canShoot = false;
        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    public abstract void ApplyDamageEffect(Player player);
}