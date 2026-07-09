using Data.HitEffects;
using Data.Stats;
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
        [SerializeField] private UnitStats unitStats;
        [SerializeField] private DamageStats damageStats;
        [SerializeField] private HitEffect[] hitEffects;
        
        public override void Use(GameObject user)
        {
            var spawnPos = user.transform.position + user.transform.forward * 2f;
            var forward = user.transform.forward;
            var barricade = GeneralSpawner.Instance.Spawn(spawnPrefab, spawnPos, Quaternion.LookRotation(forward));
            barricade.GetComponent<BarrelInstance>().Init(user, unitStats, damageStats, hitEffects);
        }
    }
}