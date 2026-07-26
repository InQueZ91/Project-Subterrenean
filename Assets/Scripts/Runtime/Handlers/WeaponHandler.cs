using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Data.Items;
using Runtime.Items;
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
        public UnityEvent<IWeapon> onWeaponUpdated;
        
        public UnityEvent<float, int> onReloadStarted; // reloadDuration, capacity
        public UnityEvent onReloadCompleted;
        public UnityEvent onReloadFailed;
    
        public Func<MagazineType, Item> onMagazineRequested; // Input(type) -> Output(Item)
        public Action<MagazineType> onMagazineConsumed; // Input(type) -> void
        
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

            // if (CurrentWeapon.Supply is IRechargeableSupply supply)
            // {
            //     supply.Recharge(Time.deltaTime);
            //     onWeaponUpdated?.Invoke(CurrentWeapon); // UI pulls charge from IRechargeableSupply
            // }
        }
        
        // Fire
        public void Fire(Vector3 direction)
        {
            var weapon = CurrentWeapon;
            if (weapon == null) return;
            if (!weapon.TryFire(_firingPoint.position, direction, gameObject)) return;
            
            onWeaponFired?.Invoke();
            onWeaponUpdated?.Invoke(weapon);
            
            // Auto-reload conventional weapons when empty
            if (weapon.Magazine is null)
                Reload();
        }
        public void SetFiringPoint(Transform point) => _firingPoint = point;

        // Reload
        public void Reload()
        {
            var weapon = CurrentWeapon;
            
            // Battery weapons don't reload
            // if (weapon.Supply is not IReloadableSupply reloadableSupply)
            // {
            //     onReloadFailed?.Invoke();
            //     return;
            // }

            if (!weapon.TryReload())
            {
                onReloadFailed?.Invoke();
                return;
            }
 
            var item = onMagazineRequested?.Invoke(weapon.Data.supportedMagazine);
            if (item == null)
            {
                weapon.CancelReload();
                onReloadFailed?.Invoke();
                return;
            }
            
            weapon.Unload();
            
            StartCoroutine(ReloadCoroutine(weapon, item.Data as MagazineItem));
        }
        
        private IEnumerator ReloadCoroutine(IWeapon weapon, MagazineItem magazineToLoad)
        {
            var reloadDuration = magazineToLoad.reloadDuration;
            onReloadStarted?.Invoke(reloadDuration, magazineToLoad.capacity);
            yield return new WaitForSeconds(reloadDuration);
            
            onReloadCompleted?.Invoke();

            weapon.FinishReload(magazineToLoad);
            
            // consume magazine item
            onMagazineConsumed?.Invoke(magazineToLoad.type);
            onWeaponUpdated?.Invoke(weapon);
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
            onWeaponUpdated?.Invoke(weapon);
        }
    }
}