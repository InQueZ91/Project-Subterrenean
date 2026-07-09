using Runtime;
using UnityEngine;

namespace Data.Items
{
    [CreateAssetMenu(fileName = "New Health Pack", menuName = "Game/Items/Health Pack")]
    public class HealthPack : UsableItemData
    {
        [SerializeField] private int healAmount = 10;
        
        public override void Use(GameObject user)
        {
            var unit = user.GetComponent<Unit>();
            if (unit == null) return;
            unit.Heal(healAmount);
        }
    }
}