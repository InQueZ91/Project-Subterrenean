using Data.Items;
using Data.Stats.Supplies;
using UnityEngine;

namespace Runtime.Weapons.Supplies
{
    public class ReloadableSupply : IReloadableSupply
    {
        public AmmoType Type { get; private set; }
        public AmmoStats Stats { get; private set;}
        public int CurrentAmmo { get; private set;}
        public int MaxAmmo => Stats.ammoCapacity;
        public bool IsReloading { get; private set;}

        public ReloadableSupply(AmmoType ammoType, AmmoStats stats)
        {
            Type = ammoType;
            Stats = stats;
            CurrentAmmo = stats.ammoCapacity;
            IsReloading = false;
        }

        public bool HasEnough()
        {
            return !IsReloading && CurrentAmmo > 0;
        }

        public void Spend()
        {
            CurrentAmmo = Mathf.Max(0, CurrentAmmo - 1);
        }

        public bool TryReload()
        {
            var ammoFull = CurrentAmmo == Stats.ammoCapacity;
            if (IsReloading || ammoFull) return false;
            
            IsReloading = true;
            return true;
        }

        public void CancelReload()
        {
            IsReloading = false;
        }

        public void FinishReload(int availableAmmo, out int ammoConsumed)
        {
            var needed = Stats.ammoCapacity - CurrentAmmo;
            ammoConsumed = Mathf.Min(availableAmmo, needed);
            CurrentAmmo += ammoConsumed;
            IsReloading = false;
        }
    }
}