using Data.HitEffects;
using Data.Stats.Output;
using UnityEngine;

namespace Runtime.Items
{
    [RequireComponent(typeof(Light))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class ClaymoreInstance : MonoBehaviour
    {
        [SerializeField] private float lightIntensity = 50f;
        [SerializeField] private MeshRenderer frontIndicatorMeshRenderer;

        // Configuration
        private LayerMask _triggerMask;
        private DamageStats _damageStats;
        private HitEffect[] _hitEffects;
        private float _triggerRadius;
        private float _activateDelayTime;
        private float _detonateDelayTime;

        // References
        private GameObject _owner;
        private SphereCollider _collider;
        private Light _light;

        // State
        private float _detonateTime;
        private float _activateTime;
        private bool _triggered;
        private bool IsActivated => Time.time >= _activateTime;

        public void Init(
            GameObject owner,
            LayerMask triggerMask,
            DamageStats damageStats,
            HitEffect[] hitEffects,
            float triggerRadius,
            float activateDelay,
            float detonateDelay)
        {
            _owner = owner;
            _triggerMask = triggerMask;
            _damageStats = damageStats;
            _hitEffects = hitEffects;
            _triggerRadius = triggerRadius;
            _activateDelayTime = activateDelay;
            _detonateDelayTime = detonateDelay;

            _activateTime = Time.time + _activateDelayTime;

            _light.range = _triggerRadius * 2;
            _collider.radius = _triggerRadius;
            _collider.center = new Vector3(0, 0, _triggerRadius);
        }

        private void Awake()
        {
            _light = GetComponent<Light>();
            _light.enabled = false;
            _light.intensity = lightIntensity;
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
        }

        private void Update()
        {
            _light.enabled = IsActivated;
            frontIndicatorMeshRenderer.enabled = IsActivated;

            if (!_triggered) return;

            var timeLeft = _detonateTime - Time.time;
            var interval = Mathf.Max(0.05f, timeLeft * 0.2f);
            _light.enabled = (int)(Time.time / interval) % 2 == 0;

            if (Time.time >= _detonateTime) Detonate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsActivated) return;

            // Ignore object that not in trigger mask
            if ((_triggerMask & (1 << other.gameObject.layer)) == 0) return;

            // Disable collider to prevent multiple hits
            _collider.enabled = false;

            // Start countdown
            _detonateTime = Time.time + _detonateDelayTime;
            _triggered = true;
        }

        private void Detonate()
        {
            if (_hitEffects != null)
            {
                var hitContext = new HitContext
                {
                    point = transform.position,
                    direction = Vector3.up,
                    victim = null,
                    owner = _owner,
                    stats = _damageStats
                };

                foreach (var effect in _hitEffects)
                    effect.Apply(hitContext);
            }

            Destroy(gameObject);
        }
    }
}