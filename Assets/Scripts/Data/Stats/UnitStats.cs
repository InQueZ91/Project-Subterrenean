using System;

namespace Data.Stats
{
    [Serializable]
    public struct UnitStats
    {
        public float maxHealth;
        public float moveSpeed;
        public float rotationSpeed;
        public float knockbackDecay;
        public float knockbackResistance;
    }
}