using System;
using UnityEngine;

namespace Data.Stats
{
    [Serializable]
    public struct FabricatorStats
    {
        [Min(1)] public int moduleSlots;
    }
}