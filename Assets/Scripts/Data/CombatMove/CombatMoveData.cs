using UnityEngine;

namespace Data.CombatMove
{
    public abstract class CombatMoveData : ScriptableObject
    {
        [Header("Timing")]
        public float activeRange;
        public float cooldown;
        public float hitWindowStart;
        public float hitWindowEnd;
        
        [Header("Interrupt")]
        public bool isInterruptable;
        public float interruptThreshold;
        
        public abstract void Execute(Vector3 origin, Vector3 direction, GameObject owner);

        /// <summary>
        /// Whether the move has fully finished.
        /// Default behavior uses the original fixed hitWindowEnd timing
        /// and only overrides when the real duration isn’t known in advance.
        /// External completion is reported by the move driver and is usually ignored.
        /// </summary>
        public virtual bool IsExecutionComplete(float attackTime, bool externalCompletionFlag)
        {
            return attackTime > hitWindowEnd;
        }
    }
}