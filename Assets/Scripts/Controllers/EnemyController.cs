using Data;
using Data.CombatMove;
using Data.Stats;
using Runtime;
using Runtime.Handlers;
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
        [SerializeField] private Transform firingPoint;
        
        // References
        private NavMeshAgent _navAgent;
        private Transform _target;
        private Unit _unit;
        private EnemyAttackHandler _attackHandler;
        
        // State
        private Vector3 _knockbackVelocity;

        [Header("Events")] 
        public UnityEvent<RewardStats, Vector3> onDied;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _navAgent = GetComponent<NavMeshAgent>();
            _attackHandler = new EnemyAttackHandler();
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

            if (_attackHandler.IsAttacking)
            {
                // Committed to an attack cycle - direction re-resolves each
                // tick so the hit lands toward wherever the target currently
                // is, not where it was at commit time.
                var direction = (_target.position - transform.position).normalized;
                _attackHandler.Tick(Time.deltaTime, firingPoint.position, direction, gameObject);
                return;
            }

            var dist = Vector3.Distance(transform.position, _target.position);
            var move = GetMoveForDistance(dist);

            if (move == null) { HandleChase(); return; }

            _navAgent.ResetPath();
            _attackHandler.BeginAttack(move);
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
            _navAgent.SetDestination(_target.position);
        }

        private CombatMoveData GetMoveForDistance(float dist)
        {
            var inRange = System.Array.FindAll(data.combatMoves, m => dist <= m.activeRange);
            return inRange.Length == 0 ? null : inRange[Random.Range(0, inRange.Length)];
        }
        
        private void OnDied()
        {
            onDied?.Invoke(data.rewards, transform.position);
            Destroy(gameObject, 0.2f);
        }

        private void OnDamageTaken(float amount, Vector3 knockback)
        {
            _knockbackVelocity = knockback / data.unitStats.knockbackResistance;
            _navAgent.ResetPath();

            var move = _attackHandler.ActiveMove;
            if (_attackHandler.IsAttacking && move != null && move.isInterruptable)
            {
                if (knockback.magnitude >= move.interruptThreshold)
                    _attackHandler.Interrupt();
            }
        }
    }
}
