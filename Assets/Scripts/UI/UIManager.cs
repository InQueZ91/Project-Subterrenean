using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weaponSlot;
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private TextMeshProUGUI wave;
    
        public void SetWeaponSlotText(string text) => weaponSlot.text = text;
        public void SetScoreText(string text) => score.text = text;
        public void SetWaveText(string text) => wave.text = text;
    }
}
