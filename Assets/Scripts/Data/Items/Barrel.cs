using Data.HitEffects;
using Data.Stats.Output;
using Runtime.Items;
using Runtime.Spawners;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "New Barrel", menuName = "Game/Items/Barrel")]
    public class Barrel : UsableItemData
    {
        [Header("Spawn")]
        [SerializeField] private GameObject spawnPrefab;
        
        [Header("Configuration")]
        [SerializeField] private float maxHealth;
        [SerializeField] private DamageStats damageStats;
        [SerializeField] private HitEffect[] hitEffects;
        
        public override void Use(GameObject user)
        {
            // Spawn barrel
            var spawnPos = user.transform.position + user.transform.forward * 2f;
            var forward = user.transform.forward;
            var barricade = GeneralSpawner.Instance.Spawn(spawnPrefab, spawnPos, Quaternion.LookRotation(forward));
            barricade.GetComponent<BarrelInstance>().Init(user, maxHealth, damageStats, hitEffects);
        }
    }
}