namespace Runtime.Weapons.Supplies
{
    public class UnlimitedSupply : IUnlimitedSupply
    {
        public bool HasEnough() => true;

        public void Spend() { }
    }
}