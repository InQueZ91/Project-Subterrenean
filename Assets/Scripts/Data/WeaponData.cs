using Data.Output;
using Data.Stats;
using Data.Supplies;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Game/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public WeaponStats stats;
        
        [Header("Supply")]
        public SupplyData supply;

        [Header("Output")] 
        public OutputData output;
        
        [Header("Visuals")]
        public GameObject visualPrefab;
        
        [Header("Audio")] 
        public AudioClip fireSound;
        public AudioClip reloadSound;
    }
}