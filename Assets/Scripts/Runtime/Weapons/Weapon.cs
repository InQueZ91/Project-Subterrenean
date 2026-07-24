using Runtime.Weapons.Supplies;
using UnityEngine;
using WeaponData = Data.WeaponData;

namespace Runtime.Weapons
{
    public class Weapon : IWeapon
    {
        // IWeapon
        public WeaponData Data { get; }
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
            
            // From rounds per minute to seconds
            var resolvedFireRate = Data.stats.fireRate;
            var toSecond = 60f / resolvedFireRate;
            NextFireTime = Time.time + toSecond;
        }
    }
}