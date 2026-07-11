using Data.HitEffects;
using Data.Stats.Output;
using Runtime.Spawners;
using UnityEngine;

namespace Data.CombatMove
{
    [CreateAssetMenu(fileName = "New Melee Move", menuName = "Game/Combat Move/Melee Move")]
    public class MeleeMoveData : CombatMoveData
    {
        [Header("Configuration")]
        public LayerMask hitMask;
        public DamageStats damage;
        public HitEffect[] hitEffects;
        public float hitboxDistance;
        public float hitboxRadius;

        public override void Execute(Vector3 origin, Vector3 direction, GameObject owner)
        {
            var hitboxData = new HitboxData(
                hitMask,
                damage,
                hitEffects,
                hitboxRadius,
                hitboxDistance,
                origin,
                direction,
                owner
            );
            
            HitboxSpawner.Instance.Spawn(hitboxData);
        }
    }
}