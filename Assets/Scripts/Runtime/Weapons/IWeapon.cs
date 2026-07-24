using Data;
using Runtime.Weapons.Supplies;

namespace Runtime.Weapons
{
    public interface IWeapon
    {
        WeaponData Data { get; }
        ISupply Supply { get; }
    
        bool CanFire();
        void Fire();
    }
}