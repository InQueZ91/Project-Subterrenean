using Data;
using Data.Items;
using UnityEngine;

namespace Runtime.Weapons
{
    public class Weapon : IWeapon
    {
        // IWeapon
        public WeaponData Data { get; private set; }
        public Magazine Magazine { get; private set; }
        
        // State
        private float nextFireTime;
        private bool isReloading;
        
        public Weapon(WeaponData data)
        {
            Data = data;
            Magazine = new Magazine(data.startingMagazine);
        }
        
        // Fire
        private bool CanFire()
        {
            return Magazine is { IsEmpty: false } && Time.time >= nextFireTime;
        }
        public bool TryFire(Vector3 origin, Vector3 direction, GameObject owner)
        {
            if (!CanFire()) return false;
            
            if (!Magazine.TryConsume())
            {
                // Publish event No Ammo then lets weapon handler load magazine
                return false;
            }
            
            Magazine.ResolvedOutput.Fire(origin, direction, owner);
            
            // From rounds per minute to seconds
            var toSecond = 60f / Data.stats.fireRate;
            nextFireTime = Time.time + toSecond;
            return true;
        }

        // Reload
        public bool TryReload()
        {
            if (isReloading) return false;
            isReloading = true;
            return true;
        }
        public void CancelReload()
        {
            isReloading = false;
        }
        public void FinishReload(MagazineItem magazine)
        {
            Magazine = new Magazine(magazine);
            isReloading = false;
        }
        public void Unload()
        {
            Magazine?.Discard();
            Magazine = null;
        }
    }
}