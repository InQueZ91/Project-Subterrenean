using Runtime.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace Visual
{
    public class WeaponHolderVisual : MonoBehaviour
    {
        private WeaponVisual _currentVisual;
        
        public UnityEvent<Transform> onFiringPointChanged = new();

        public void SwitchWeaponTo(IWeapon newWeapon)
        {
            // Destroy old visual
            if (_currentVisual != null)
                Destroy(_currentVisual.gameObject);

            _currentVisual = null;

            var weaponData = newWeapon.Data;
            if (weaponData.visualPrefab == null)
            {
                Debug.LogWarning($"WeaponHolderVisual: weapon has no visualPrefab.", gameObject);
                return;
            }

            var go = Instantiate(weaponData.visualPrefab, transform);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (!go.TryGetComponent(out _currentVisual))
            {
                Debug.LogError($"WeaponHolderVisual: visualPrefab '{go.name}' has no WeaponVisual component.", go);
                Destroy(go);
                return;
            }

            _currentVisual.Init(weaponData);
            onFiringPointChanged?.Invoke(_currentVisual.GetFirePoint);
        }
        
        public void OnReloadStarted()
        {
            if (_currentVisual == null) return;
            _currentVisual.PlayReload();
        }

        public void OnWeaponFired()
        {
            if (_currentVisual == null)
            {
                Debug.LogWarning("WeaponHolderVisual: no visual set, cannot fire.");
                return;
            }

            _currentVisual.PlayFire();
        }
    }
}