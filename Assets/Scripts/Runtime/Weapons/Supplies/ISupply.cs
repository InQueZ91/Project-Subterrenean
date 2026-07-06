namespace Runtime.Weapons.Supplies
{
    public interface ISupply
    {
        bool HasEnough();
        void Spend();
    }
}