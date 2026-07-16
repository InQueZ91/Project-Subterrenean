using Data.HitEffects;
using Data.Stats;
using Data.Stats.Output;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Items
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshObstacle))]
    public class BarrelInstance : MonoBehaviour
    {
        private GameObject _user;
        private Unit _unit;
        private DamageStats _damageStats;
        private HitEffect[] _hitEffects;
        
        private NavMeshObstacle _navMeshObstacle;

        public void Init(GameObject user, UnitStats unitStats, DamageStats damageStats, HitEffect[] hitEffects)
        {
            _user = user;
            _damageStats = damageStats;
            _hitEffects = hitEffects;
            
            _unit = GetComponent<Unit>();
            _unit.Init(unitStats);
            _unit.onDied.AddListener(OnDied);
            
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            _navMeshObstacle.carving = true;
        }

        public void OnDied()
        {
            var hitContext = new HitContext
            {
                point = transform.position,
                direction = Vector3.up,
                victim = null,
                owner = _user,
                stats = _damageStats
            };

            foreach (var effect in _hitEffects)
                effect.Apply(hitContext);
            
            Destroy(gameObject);
        }
    }
}