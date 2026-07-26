using Data.Items;
using Data.Output;

namespace Runtime.Weapons
{
    public class Magazine
    {
        // Static data cached from magazine item.
        public MagazineItem Data { get; private set; }
        public OutputData ResolvedOutput { get; private set; }

        // State
        public int CurrentRounds { get; private set; }
        public bool IsEmpty => CurrentRounds <= 0;

        public Magazine(MagazineItem magazineData)
        {
            Data = magazineData;
            ResolvedOutput = Data.outputData;
            CurrentRounds = Data.capacity;
        }

        public bool TryConsume()
        {
            if (IsEmpty) return false;
            CurrentRounds--;
            return true;
        }
        
        // Weapon destroyed or combined - caller just drops the reference, GC handles it
        public void Discard() { } //explicit no-op, documents intent
    }
}