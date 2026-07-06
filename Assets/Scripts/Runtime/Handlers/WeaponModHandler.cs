using System.Collections.Generic;
using Data.Mods;
using Data.Stats;
using Data.Stats.Output;

namespace Runtime.Handlers
{
    public class WeaponModHandler
    {
        private readonly List<WeaponMod> _mods = new();
        
        public void Add(WeaponMod mod) => _mods.Add(mod);
        public void Remove(WeaponMod mod) => _mods.Remove(mod);

        public T Resolve<T>(T baseStats) where T : struct
        {
            foreach (var mod in _mods)
            {
                if (mod is IModifies<T> modifier)
                    modifier.Apply(ref baseStats);
            }

            return baseStats;
        }
    }
}