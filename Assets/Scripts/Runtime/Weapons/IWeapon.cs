using Data;
using Runtime.Handlers;
using Runtime.Weapons.Supplies;

namespace Runtime.Weapons
{
    public interface IWeapon
    {
        WeaponData Data { get; }
        WeaponModHandler Mods { get; }
        ISupply Supply { get; }
    
        bool CanFire();
        void Fire();
    }
}