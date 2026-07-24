using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Data.Items;
using Runtime.Weapons;
using Runtime.Weapons.Supplies;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Handlers
{
    [Serializable]
    public class WeaponHandler : MonoBehaviour
    {
        // Runtime
        [SerializeField] private int currentWeaponIndex;
        private readonly List<IWeapon> _weapons = new();
        
        private Transform _firingPoint;
        
        public int CurrentWeaponIndex => currentWeaponIndex;
        public IWeapon CurrentWeapon => _weapons.Count > 0 ? _weapons[currentWeaponIndex] : null;

        [Header("Events")]
        public UnityEvent onWeaponFired;
        public UnityEvent<IWeapon> onWeaponEquipped;
        public UnityEvent<IWeapon> onAmmoChanged;
        
        public UnityEvent<float, int, int> onReloadStarted; // reloadTimePerAmmo, ammoToLoad, currentAmmo
        public UnityEvent onReloadCompleted;
        public UnityEvent onReloadFailed;

        public Func<AmmoType, int> onAmmoRequested; // AmmoType → available reserve count
        public Func<AmmoType, int, int> onAmmoConsumed; // AmmoType, needed → actually consumed
        
        // Initialization
        public void Init(WeaponData[] startingWeapons)
        {
            _weapons.Clear();
            foreach (var weaponData in startingWeapons)
                _weapons.Add(new Weapon(weaponData));

            if (_weapons.Count > 0)
                EquipWeapon(0);
        }

        // Unity lifecycle
        private void Update()
        {
            // Recharge battery weapons every frame for passive recharge

            if (CurrentWeapon.Supply is IRechargeableSupply supply)
            {
                supply.Recharge(Time.deltaTime);
                onAmmoChanged?.Invoke(CurrentWeapon); // UI pulls charge from IRechargeableSupply
            }
        }
        
        // Fire
        public void Fire(Vector3 direction)
        {
            var weapon = CurrentWeapon;
            if (weapon == null || !weapon.CanFire()) return;
            
            weapon.Fire();
            
            weapon.Data.output.Fire(_firingPoint.position, direction, gameObject);
            
            onWeaponFired?.Invoke();
            onAmmoChanged?.Invoke(weapon);
            
            // Auto-reload conventional weapons when empty
            if (weapon.Supply is IReloadableSupply { CurrentAmmo: <= 0})
                Reload();
        }

        public void SetFiringPoint(Transform point) => _firingPoint = point;

        // Reload
        public void Reload()
        {
            var weapon = CurrentWeapon;
            
            // Battery weapons don't reload
            if (weapon.Supply is not IReloadableSupply reloadableSupply)
            {
                onReloadFailed?.Invoke();
                return;
            }

            if (!reloadableSupply.TryReload())
            {
                onReloadFailed?.Invoke();
                return;
            }

            StartCoroutine(ReloadCoroutine(weapon, reloadableSupply));
        }
        
        private IEnumerator ReloadCoroutine(IWeapon weapon, IReloadableSupply supply)
        {
            var ammoType = supply.Type;
            var availableReserve = onAmmoRequested?.Invoke(ammoType) ?? 0;

            if (availableReserve <= 0)
            {
                supply.CancelReload();
                onReloadFailed?.Invoke();
                yield break;
            }

            var resolvedStats = supply.Stats;
            var ammoToLoad = Mathf.Min(availableReserve, resolvedStats.ammoCapacity - supply.CurrentAmmo);
            var reloadTimePerAmmo = resolvedStats.reloadTime / ammoToLoad;
            
            onReloadStarted?.Invoke(reloadTimePerAmmo, ammoToLoad, supply.CurrentAmmo);

            yield return new WaitForSeconds(resolvedStats.reloadTime);
            
            onReloadCompleted?.Invoke();

            supply.FinishReload(availableReserve, out var consumed);
            
            onAmmoConsumed?.Invoke(ammoType, consumed);
            onAmmoChanged?.Invoke(weapon);
        }

        // Switch
        public void SwitchWeapon(int index)
        {
            if (_weapons.Count == 0) return;
            currentWeaponIndex = ((index % _weapons.Count) + _weapons.Count) % _weapons.Count;
            EquipWeapon(currentWeaponIndex);
        }

        private void EquipWeapon(int index)
        {
            if (_weapons.Count <= 0) return;

            var weapon = _weapons[index];
            onWeaponEquipped?.Invoke(weapon);
            onAmmoChanged?.Invoke(weapon);
        }
    }
}