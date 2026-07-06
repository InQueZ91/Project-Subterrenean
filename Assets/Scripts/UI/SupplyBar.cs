using System.Collections.Generic;
using Runtime.Weapons;
using Runtime.Weapons.Supplies;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SupplyBar : MonoBehaviour
    {
        [SerializeField] private Animator reloadAnimator;
        [SerializeField] private GameObject pipPrefab;
        [SerializeField] private Transform pipContainer;
        [SerializeField] private int supplyPerPip = 4; // how many ammo units one pip represents

        // ── reload tick state ────────────────────────────────────────────────────
        private float _reloadTimePerAmmo; // seconds between each ammo unit
        private float _reloadElapsed;     // time accumulated since reload started
        private bool _isReloading;
        private float _reloadStartAmmo;   // ammo value when this reload began
        private float _reloadTargetAmmo;  // ammo value when this reload finishes

        // ── display state ────────────────────────────────────────────────────────
        private IWeapon _currentWeapon;
        private readonly List<Image> _pipFills = new();
        private bool _isRechargeableMode;

        // ── Equip ─────────────────────────────────────────────────────────────────

        public void RebuildPips(IWeapon weapon)
        {
            _currentWeapon = weapon;
            _isReloading = false;
            _isRechargeableMode = weapon.Supply is IRechargeableSupply;

            foreach (Transform child in pipContainer)
                Destroy(child.gameObject);
            _pipFills.Clear();

            // Battery: always a single pip representing charge %.
            // Reloadable: one pip per ammoPerPip units of MaxAmmo.
            // Unlimited (melee): also a single pip, RefreshPips just won't touch it.
            var pipCount = weapon.Supply is IReloadableSupply reloadable
                ? Mathf.CeilToInt((float)reloadable.MaxAmmo / supplyPerPip)
                : 1;

            for (var i = 0; i < pipCount; i++)
            {
                var pip = Instantiate(pipPrefab, pipContainer);
                var ammoFill = pip.GetComponent<SupplyPip>().ammoFill;
                ammoFill.fillAmount = 1f;
                _pipFills.Add(ammoFill);
            }

            RefreshPips(weapon); // paint initial state immediately (full battery, full ammo, etc.)
        }

        // ── Live updates ─────────────────────────────────────────────────────────

        public void RefreshPips(IWeapon weapon)
        {
            if (_currentWeapon?.Data != weapon.Data) return;

            if (weapon.Supply is IRechargeableSupply rechargeable)
            {
                UpdateBatteryFill(rechargeable);
                return;
            }

            _isReloading = false;

            if (weapon.Supply is IReloadableSupply reloadable)
                UpdatePipsFill(reloadable.CurrentAmmo);
        }

        private void UpdateBatteryFill(IRechargeableSupply rechargeable)
        {
            if (_pipFills.Count < 1) return;
            if (rechargeable.MaxCharge <= 0f) return;

            _pipFills[0].fillAmount = rechargeable.CurrentCharge / rechargeable.MaxCharge;
        }

        private void UpdatePipsFill(float ammo)
        {
            for (var i = 0; i < _pipFills.Count; i++)
            {
                var pipStartAmmo = i * supplyPerPip;
                var pipFillAmount = Mathf.Clamp(ammo - pipStartAmmo, 0f, supplyPerPip);
                _pipFills[i].fillAmount = pipFillAmount / supplyPerPip;
            }
        }

        // ── Reload lifecycle (conventional weapons only — batteries never reload) ──

        public void StartReload(float reloadTimePerAmmo, int ammoToLoad, int currentAmmo)
        {
            if (_isRechargeableMode) return;

            _reloadTimePerAmmo = reloadTimePerAmmo;
            _isReloading = true;
            _reloadElapsed = 0f;
            _reloadStartAmmo = currentAmmo;
            _reloadTargetAmmo = currentAmmo + ammoToLoad;

            reloadAnimator.Play("Reload");
        }

        public void CompleteReload()
        {
            if (_isRechargeableMode) return;
            reloadAnimator.Play("ReloadComplete");
        }

        public void FailReload()
        {
            if (_isRechargeableMode) return;
            reloadAnimator.Play("ReloadFail");
        }

        private void Update()
        {
            TickReload();
        }

        private void TickReload()
        {
            if (!_isReloading) return;
            if (_pipFills.Count < 1) return;

            _reloadElapsed += Time.deltaTime;
            
            var progress = _reloadTimePerAmmo > 0f ? _reloadElapsed / _reloadTimePerAmmo : 1f;
            var currentAmmo = Mathf.Min(_reloadStartAmmo + progress, _reloadTargetAmmo);

            UpdatePipsFill(currentAmmo);

            if (currentAmmo >= _reloadTargetAmmo)
                _isReloading = false;
        }
    }
}