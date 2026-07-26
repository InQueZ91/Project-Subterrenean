using Runtime.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WeaponSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private Image weaponIcon;

        private void Awake()
        {
            ammoText.text = "0";
        }

        public void SetAmmo(int ammo)
        {
            ammoText.text = ammo.ToString();
        }

        public void SwitchWeapon(Weapon newWeapon)
        {
            SetAmmo(newWeapon.Magazine.CurrentRounds);
            // if (newWeapon.Supply is IReloadableSupply reloadableSupply)
            // {
            //     SetAmmo(reloadableSupply.CurrentAmmo);
            // }
            // else
            // {
            //     ammoText.text = "";
            // }
        }
    }
}
