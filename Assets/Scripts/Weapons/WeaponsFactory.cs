using UnityEngine;

public class WeaponsFactory : IWeaponsFactory
{
    private readonly WeaponsConfiguration _weaponsConfiguration;

    public WeaponsFactory(WeaponsConfiguration weaponsConfiguration)
    {
        this._weaponsConfiguration = weaponsConfiguration;
    }
        
    public WeaponBase Create(string id)
    {
        var prefab = _weaponsConfiguration.GetWeaponPrefabById(id);

        return Object.Instantiate(prefab);
    }
}

public interface IWeaponsFactory
{
    WeaponBase Create(string id);
}