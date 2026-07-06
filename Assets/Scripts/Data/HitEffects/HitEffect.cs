using System;
using UnityEngine;

namespace Data.HitEffects
{
    [Serializable]
    public abstract class HitEffect : ScriptableObject
    {
        public GameObject hitVFX;
        public bool isFriendlyFire = false;
        
        public abstract void Apply(HitContext ctx);
    }
}