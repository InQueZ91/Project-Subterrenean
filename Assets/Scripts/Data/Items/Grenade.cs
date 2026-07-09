using Data.HitEffects;
using Data.Stats.Output;
using Runtime.Items;
using Runtime.Spawners;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "New Grenade", menuName = "Game/Items/Grenade")]
    public class Grenade : UsableItemData
    {
        [Header("Spawn")]
        [SerializeField] private GameObject spawnPrefab;
 
        [Header("Throw")]
        [SerializeField] private float throwForce = 10f;
        [SerializeField] private float throwingAngle = 30f;
 
        [Header("Configuration")]
        [SerializeField] private DamageStats stats;
        [SerializeField] private HitEffect[] hitEffects;
        [SerializeField] private float fuseTime = 3f;
 
        public override void Use(GameObject user)
        {
            var origin = user.transform.position + user.transform.forward * 0.5f;
            var forward = user.transform.forward;
            var direction = Quaternion.AngleAxis(-throwingAngle, user.transform.right) * forward;
 
            var go = GeneralSpawner.Instance.Spawn(spawnPrefab, origin, Quaternion.identity);
            go.GetComponent<GrenadeInstance>().Init(
                owner: user,
                damageStats: stats,
                hitEffects: hitEffects,
                fuseTime: fuseTime,
                throwVelocity: direction * throwForce
            );
        }
    }
}