namespace Data.Mods
{
    public interface IModifies<T>
    {
        void Apply(ref T stats);
    }
}