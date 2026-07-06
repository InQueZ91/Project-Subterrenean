using Data.Stats;
using Data.Stats.Output;
using UnityEngine;

namespace Data.HitEffects
{
    public struct HitContext
    {
        public Vector3 point;
        public Vector3 direction;
        public GameObject victim;
        public GameObject owner;
        public DamageStats stats;
    }
}