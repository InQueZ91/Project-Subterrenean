using Data.Items;
using Data.Stats.Supplies;

namespace Runtime.Weapons.Supplies
{
    public interface IReloadableSupply : ISupply
    {
        AmmoType Type { get; }
        AmmoStats Stats { get; }
        int CurrentAmmo { get; }
        int MaxAmmo { get; }
        bool IsReloading { get; }
        bool TryReload();
        void CancelReload();
        void FinishReload(int availableAmmo, out int ammoConsumed);
    }
}