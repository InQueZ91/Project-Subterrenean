using Data.Output;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "New Magazine", menuName = "Game/Items/Magazine")]
    public class MagazineItem : ItemData
    {
        [Header("MagazineData")]
        public MagazineType type;
        public int capacity;
        public float reloadDuration;
        public OutputData outputData;
    }
}