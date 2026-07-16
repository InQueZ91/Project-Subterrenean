using Data.CombatMove;
using Data.Stats;
using UnityEngine;

namespace Data.Bosses
{
    [CreateAssetMenu(fileName = "New Boss", menuName = "Game/Bosses/Boss")]
    public class BossData : ScriptableObject
    {
        [Header("Core")]
        public UnitStats unitStats;
        public RewardStats rewards;
        public float visionAngle = 30f;
        
        [Header("Normal Phase")]
        public CombatMoveData normalMove;
        
        [Header("Enraged Phase (below enrageHealthPercent)")]
        [Range(0f, 1f)] public float enrageHealthPercent = 0.5f;
        public ChargeMoveData chargeMove;
        public RangeMoveData rangeMove;
        
        [Header("Ultimate Move")]
        [Tooltip("Health percent thresholds that trigger the ultimate move")]
        public float[] ultimateHealthThresholds = {0.25f, 0.1f};
        [Tooltip("The 3 escalating slam stages executed in sequence when triggered.")]
        public MeleeMoveData[] superSlamStages = new MeleeMoveData[3];
    }
}