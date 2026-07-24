using Data;
using Data.Stats;
using Runtime;
using Runtime.Handlers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(WeaponHandler))]
    [RequireComponent(typeof(InventoryHandler))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterData data;

        [Header("References")] 
        [SerializeField] private Transform cameraTransform;

        // Private state
        private Camera _cam;
        private CharacterController _cc;
        private PlayerInputActions _actions;
        private Vector2 _moveInput;

        private Unit _unit;
        private WeaponHandler _weaponHandler;
        private InventoryHandler _inventoryHandler;
        
        private Vector3 _knockbackVelocity;

        private readonly Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);

        // Unity
        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _cam = Camera.main;
            _actions = new PlayerInputActions();

            _unit = GetComponent<Unit>();
            _weaponHandler = GetComponent<WeaponHandler>();
            _inventoryHandler = GetComponent<InventoryHandler>();

            if (cameraTransform == null && _cam != null)
                cameraTransform = _cam.transform;
        }

        private void Start()
        {
            _unit.Init(data.unitStats.maxHealth);
            _unit.onDamageTaken.AddListener(OnDamageTaken);
            _unit.onDied.AddListener(OnDied);
            
            _weaponHandler.Init(data.startingWeapons);
            _inventoryHandler.Init(data.startingItems);
            
            // Wire
            _weaponHandler.onAmmoRequested = _inventoryHandler.GetAmmoCount;
            _weaponHandler.onAmmoConsumed = _inventoryHandler.ConsumeAmmo;
        }

        private void OnEnable()
        {
            _actions.Enable();
        }

        private void OnDisable()
        {
            _unit.onDamageTaken.RemoveListener(OnDamageTaken);
            _actions.Disable();
        }

        private void Update()
        {
            HandleMovement();
            HandleAiming();
            HandleShooting();
            HandleSwitchingWeapon();
            HandleSwitchingItem();
            HandleUseItem();
            HandleDropItem();
        }
        
        private void OnDamageTaken(float amount, Vector3 knockback)
        {
            // Flatten knockback to XZ — Y component causes floating with CharacterController
            knockback.y = 0f;
            _knockbackVelocity = knockback / data.unitStats.knockbackResistance;
        }

        private void OnDied()
        {
            Debug.Log("Player has died.");
        }

        // Movement
        private void HandleMovement()
        {
            _knockbackVelocity = Vector3.MoveTowards(
                _knockbackVelocity,
                Vector3.zero,
                data.unitStats.knockbackDecay * Time.deltaTime
            );

            _moveInput = _actions.Player.Move.ReadValue<Vector2>();
            var camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            var camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            var moveDir = (camForward * _moveInput.y + camRight * _moveInput.x).normalized;

            if (_knockbackVelocity.sqrMagnitude > 0.01f) moveDir = Vector3.zero;

            var motion = moveDir * (data.unitStats.moveSpeed * Time.deltaTime);
            motion += _knockbackVelocity * Time.deltaTime;
            motion.y = -2f * Time.deltaTime; // constant ground-glue, no jumping

            _cc.Move(motion);
        }

        // Weapon
        private void HandleAiming()
        {
            if (Mouse.current == null) return;

            // Cast a ray from the mouse position onto the ground plane
            var ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!_groundPlane.Raycast(ray, out var enter)) return;

            var dir = ray.GetPoint(enter) - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(dir),
                data.unitStats.rotationSpeed * Time.deltaTime
            );
        }

        private void HandleShooting()
        {
            var weapon = _weaponHandler.CurrentWeapon;
            if (weapon == null) return;

            // Auto-fire weapons: hold button. Manual: press only.
            var shouldFire = weapon.Data.stats.isAutomatic
                ? _actions.Player.Fire.IsPressed()
                : _actions.Player.Fire.WasPressedThisFrame();

            if (shouldFire)
                _weaponHandler.Fire(transform.forward);

            if (_actions.Player.Reload.WasPressedThisFrame())
                _weaponHandler.Reload();
        }

        private void HandleSwitchingWeapon()
        {
            if (_weaponHandler.CurrentWeapon == null) return;

            if (_actions.Player.NextWeapon.WasPressedThisFrame())
                _weaponHandler.SwitchWeapon(_weaponHandler.CurrentWeaponIndex + 1);

            if (_actions.Player.PreviousWeapon.WasPressedThisFrame())
                _weaponHandler.SwitchWeapon(_weaponHandler.CurrentWeaponIndex - 1);
        }

        // Item
        private void HandleUseItem()
        {
            if (_inventoryHandler.CurrentItem == null) return;

            if (_actions.Player.UseItem.WasPressedThisFrame())
                _inventoryHandler.UseItem(gameObject);
        }
        
        private void HandleDropItem()
        {
            if (_inventoryHandler.CurrentItem == null) return;

            if (_actions.Player.DropItem.WasPressedThisFrame())
                _inventoryHandler.DropItem();
        }
        
        private void HandleSwitchingItem()
        {
            if (_inventoryHandler.CurrentItem == null) return;

            if (_actions.Player.NextItem.WasPressedThisFrame())
                _inventoryHandler.SwitchItem(_inventoryHandler.CurrentItemIndex + 1);

            if (_actions.Player.PreviousItem.WasPressedThisFrame())
                _inventoryHandler.SwitchItem(_inventoryHandler.CurrentItemIndex - 1);
        }
    }
}