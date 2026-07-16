using Data.Stats;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Game/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public UnitStats unitStats;
        public WeaponData[] startingWeapons;
        public StartingItemData[] startingItems;
    }
}