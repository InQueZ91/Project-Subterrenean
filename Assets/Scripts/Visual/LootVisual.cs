using Data.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visual
{
    public class LootVisual : MonoBehaviour
    {
        [SerializeField] private float destroyAfterSeconds = 1f;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI collectText;
        [SerializeField] private Animator lootAnimator;

        private void Start()
        {
            collectText.text = "";
        }

        public void OnCollected(ItemData itemData, int quantity)
        {
            collectText.text = $"+{quantity}";
            itemImage.sprite = itemData.icon;
            lootAnimator.Play($"Show");
            
            Destroy(gameObject, destroyAfterSeconds);
        }
    }
}