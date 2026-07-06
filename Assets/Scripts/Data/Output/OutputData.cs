using Runtime.Handlers;
using UnityEngine;

namespace Data.Output
{
    /// <summary>
    /// Base for "what a weapon or item produces when used."
    /// Shared and neutral - not owned by weapons specifically,
    /// so usable items (e.g. grenade, claymore) can reference these same types.
    /// </summary>
    public abstract class OutputData : ScriptableObject
    {
        public abstract void Fire(WeaponModHandler mods, Vector3 origin, Vector3 direction, GameObject owner);
    }
}