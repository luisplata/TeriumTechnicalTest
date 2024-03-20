using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Custom/WeaponBase configuration")]
public class WeaponsConfiguration : ScriptableObject
{
    [SerializeField] private WeaponBase[] weapons;
    private Dictionary<string, WeaponBase> idToWeapon;

    private void Awake()
    {
        idToWeapon = new Dictionary<string, WeaponBase>(weapons.Length);
        foreach (var weapon in weapons)
        {
            idToWeapon.Add(weapon.Id, weapon);
        }
    }

    public WeaponBase GetWeaponPrefabById(string id)
    {
        if (!idToWeapon.TryGetValue(id, out var weapon))
        {
            throw new Exception($"Weapon with id {id} does not exit");
        }
        return weapon;
    }
}