using Data;
using Data.Items;
using UnityEngine;

namespace Runtime.Weapons
{
    public interface IWeapon
    {
        WeaponData Data { get; }
        Magazine Magazine { get; }
    
        bool TryFire(Vector3 origin, Vector3 direction, GameObject owner);
        bool TryReload();
        void CancelReload();
        void FinishReload(MagazineItem magazine);
        void Unload();
    }
}