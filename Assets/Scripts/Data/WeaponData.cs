using Data.Items;
using Data.Stats;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Game/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public WeaponStats stats;
        public MagazineType supportedMagazine;
        public MagazineItem startingMagazine;
        
        [Header("Visuals")]
        public GameObject visualPrefab;
        
        [Header("Audio")] 
        public AudioClip fireSound;
        public AudioClip reloadSound;
    }
}