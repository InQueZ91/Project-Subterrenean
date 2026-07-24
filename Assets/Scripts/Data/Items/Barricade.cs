using Runtime.Items;
using Runtime.Spawners;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "New Barricade", menuName = "Game/Items/Barricade")]
    public class Barricade : UsableItemData
    {
        [Header("Spawn")]
        [SerializeField] private GameObject spawnPrefab;
        
        [Header("Configuration")]
        [SerializeField] private float health = 100f;
        
        public override void Use(GameObject user)
        {
            var spawnPos = user.transform.position + user.transform.forward * 2f;
            var forward = user.transform.forward;
            var barricade = GeneralSpawner.Instance.Spawn(spawnPrefab, spawnPos, Quaternion.LookRotation(forward));
            barricade.GetComponent<BarricadeInstance>().Init(health);
        }
    }
}