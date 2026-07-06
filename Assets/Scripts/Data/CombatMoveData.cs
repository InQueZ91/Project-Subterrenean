using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Combat Move", menuName = "Game/Combat Move", order = 0)]
    public class CombatMoveData : ScriptableObject
    {
        [Header("Timing")]
        public float range;
        public float cooldown;
        
        [Header("Damage")]
        public float damage;
        public float knockbackStrength;

        [Header("Output")] 
        public CombatType type;
        
        [Header("Visuals")]
        public GameObject visualPrefab;
        public AudioClip sfx;

    }

    public enum CombatType
    {
        Melee,
        Range
    };
}