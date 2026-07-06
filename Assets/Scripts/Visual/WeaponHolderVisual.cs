using Runtime.Weapons;
using UnityEngine;

namespace Visual
{
    public class WeaponHolderVisual : MonoBehaviour
    {
        private WeaponVisual _currentVisual;

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
        }

        public void OnReloadStarted()
        {
            if (_currentVisual == null) return;
            _currentVisual.PlayReload();
        }

        public void OnWeaponFired(FiringContext ctx)
        {
            if (_currentVisual == null)
            {
                Debug.LogWarning("WeaponHolderVisual: no visual set, cannot fire.");
                return;
            }

            _currentVisual.PlayFire();

            var firePoint = _currentVisual.GetFirePoint;
            if (firePoint == null)
            {
                Debug.LogWarning("WeaponHolderVisual: no fire point on WeaponVisual.", _currentVisual);
                return;
            }

            ctx.Output.Fire(ctx.Mods, firePoint.position, ctx.Direction, ctx.Owner);
        }
    }
}