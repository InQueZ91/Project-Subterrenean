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

        [Header("Visuals")]
        public GameObject visualPrefab;
        public AudioClip sfx;
        
        public abstract void Execute(Vector3 origin, Vector3 direction, GameObject owner);
    }
}