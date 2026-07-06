using Data.Stats.Supplies;

namespace Runtime.Weapons.Supplies
{
    public interface IRechargeableSupply : ISupply
    {
        BatteryStats Stats { get; }
        float CurrentCharge { get; }
        float MaxCharge { get; }
        void Recharge(float deltaTime);
    }
}