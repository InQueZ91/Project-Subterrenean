using Data.Output;
using Runtime.Handlers;
using UnityEngine;

namespace Runtime.Weapons
{
    public struct FiringContext
    {
        public OutputData Output { get; }
        public WeaponModHandler Mods { get; }
        public Vector3 Direction { get; }
        public GameObject Owner { get; }
        
        public FiringContext(OutputData output, WeaponModHandler mods, Vector3 direction, GameObject owner)
        {
            Output = output;
            Mods = mods;
            Direction = direction;
            Owner = owner;
        }
    }
}