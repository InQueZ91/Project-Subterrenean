using Data.Stats;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public UnitStats unitStats;
        // Behavior
        public CombatMoveData[] combatMoves;
        public RewardStats rewards;
    }
}