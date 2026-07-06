using Data;
using Data.Stats;
using Runtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Controllers
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
    
        // References
        private NavMeshAgent _navAgent;
        private Transform _target;
        private Unit _unit;
        
        // State
        private float _attackTimer;
        private Vector3 _knockbackVelocity;
        private CombatMoveData _activeCombatMove;
        private bool _inCombat;

        [Header("Events")] 
        public UnityEvent<RewardStats, Vector3> onDied;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _navAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _navAgent.speed = data.unitStats.moveSpeed;
            _unit.Init(data.unitStats);
            _unit.onDamageTaken.AddListener(OnDamageTaken);
            _unit.onDied.AddListener(OnDied);
        
            // For now: find the player directly.
            // Later: EnemyManager will assign targets
            var player = GameObject.FindWithTag("Player");
            if (player != null) _target = player.transform;
        }

        private void OnDisable()
        {
            _unit.onDamageTaken.RemoveListener(OnDamageTaken);
            _unit.onDied.RemoveListener(OnDied);
        }

        private void Update()
        {
            if (!_navAgent.isOnNavMesh) return;
            if (HandleKnockback()) return;
            if (_target == null) return;

            var dist = Vector3.Distance(transform.position, _target.position);
            var move = GetMoveForDistance(dist);

            if (move == null) { HandleChase(); return; }

            HandleCombat(move, dist);
        }
        
        private bool HandleKnockback()
        {
            _knockbackVelocity = Vector3.MoveTowards(
                _knockbackVelocity,
                Vector3.zero,
                data.unitStats.knockbackDecay * Time.deltaTime
            );

            if (_knockbackVelocity.sqrMagnitude <= 0.01f) return false;

            _navAgent.Move(_knockbackVelocity * Time.deltaTime);
            return true;
        }

        private void HandleChase()
        {
            _inCombat = false;
            _activeCombatMove = null;
            _navAgent.SetDestination(_target.position);
        }

        private void HandleCombat(CombatMoveData move, float dist)
        {
            if (!_inCombat)
            {
                _inCombat = true;
                _activeCombatMove = move;
                _navAgent.ResetPath();
            }

            _attackTimer -= Time.deltaTime;
            if (_attackTimer > 0f) return;

            var knockbackDir = (_target.position - transform.position - Vector3.up * 0.3f).normalized;
            _target.GetComponent<IDamageable>()
                ?.TakeDamage(_activeCombatMove.damage, knockbackDir * _activeCombatMove.knockbackStrength);

            _attackTimer = _activeCombatMove.cooldown;
            _activeCombatMove = GetMoveForDistance(dist);
        }

        private CombatMoveData GetMoveForDistance(float dist)
        {
            var inRange = System.Array.FindAll(data.combatMoves, m => dist <= m.range);
            return inRange.Length == 0 ? null : inRange[Random.Range(0, inRange.Length)];
        }
        
        private void OnDied() => onDied?.Invoke(data.rewards, transform.position);

        private void OnDamageTaken(float amount, Vector3 knockback)
        {
            _knockbackVelocity = knockback / data.unitStats.knockbackResistance;
            _navAgent.ResetPath();
        }
    }
}
