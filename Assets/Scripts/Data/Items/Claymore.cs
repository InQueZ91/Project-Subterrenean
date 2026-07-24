using Data.HitEffects;
using Data.Stats.Output;
using Runtime.Items;
using Runtime.Spawners;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "New Claymore", menuName = "Game/Items/Claymore")]
    public class Claymore : UsableItemData
    {
        [Header("Spawn")]
        [SerializeField] private GameObject spawnPrefab;
        
        [Header("Configuration")]
        [SerializeField] private LayerMask triggerMask;
        [SerializeField] private DamageStats damageStats;
        [SerializeField] private HitEffect[] hitEffects;
        [SerializeField] private float triggerRadius = 1f;
        [SerializeField] private float activateDelay = 1f;
        [SerializeField] private float detonateDelay = 1f;
        
        public override void Use(GameObject user)
        {
            // Spawn claymore
            var origin = user.transform.position + user.transform.forward;
            var forward = user.transform.forward;
            var claymore = GeneralSpawner.Instance.Spawn(spawnPrefab, origin, Quaternion.LookRotation(forward));
            claymore.GetComponent<ClaymoreInstance>().Init(
                owner: user,
                triggerMask: triggerMask,
                damageStats: damageStats,
                hitEffects: hitEffects,
                triggerRadius: triggerRadius,
                activateDelay: activateDelay,
                detonateDelay: detonateDelay
            );
        }
    }
}