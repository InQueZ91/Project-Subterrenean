using Runtime.Handlers;
using Runtime.Weapons.Supplies;
using UnityEngine;
using WeaponData = Data.WeaponData;

namespace Runtime.Weapons
{
    public class Weapon : IWeapon
    {
        // IWeapon
        public WeaponData Data { get; }
        public WeaponModHandler Mods { get; } = new();
        public ISupply Supply { get; }
        
        // Runtime
        public float NextFireTime { get; private set; }

        public Weapon(WeaponData data)
        {
            Data = data;
            Supply = data.supply.CreateSupplyState();
        }

        // IWeapon
        public bool CanFire() => Supply.HasEnough() && Time.time >= NextFireTime;

        public void Fire()
        {
            if (!CanFire()) return;

            Supply.Spend();
            NextFireTime = Time.time + Data.stats.fireRate;
        }
    }
}