using System.Linq;
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
    [RequireComponent(typeof(EnemyMovementHandler))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        [SerializeField] private Transform firingPoint;
        
        // References
        private Transform _target;
        private Unit _unit;
        private EnemyAttackHandler _attackHandler;
        private EnemyMovementHandler _movementHandler;
        
        // State
        private Vector3 _knockbackVelocity;

        [Header("Events")] 
        public UnityEvent<RewardStats, Vector3> onDied;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _attackHandler = new EnemyAttackHandler();
            _movementHandler = GetComponent<EnemyMovementHandler>();
        }

        private void Start()
        {
            _unit.Init(data.unitStats);
            _unit.onDamageTaken.AddListener(OnDamageTaken);
            _unit.onDied.AddListener(OnDied);
            
            _movementHandler.Init(data.unitStats);
        
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
            if (_target == null) return;
            
            if (_movementHandler.IsStaggered()) return;

            if (_attackHandler.IsAttacking)
            {
                var direction = (_target.position - transform.position).normalized;
                _attackHandler.Tick(Time.deltaTime, firingPoint.position, direction, gameObject);
                return;
            }

            HandleMoveSelection();
        }

        #region Move Selection

        private void HandleMoveSelection()
        {
            var candidates = data.combatMoves.ToList().FindAll(IsInRange);
            var selectedMove = candidates.Count == 0 ? null : candidates[Random.Range(0, candidates.Count)];

            if (selectedMove == null)
            {
                _movementHandler.MoveTo(_target.position);
                return;
            }
            
            _movementHandler.Stop();
            _attackHandler.BeginAttack(selectedMove);
        }
        
        private bool IsInRange(CombatMoveData move)
        {
            if (move == null) return false;

            var distanceToTarget = Vector3.Distance(transform.position, _target.position);
            return distanceToTarget <= move.activeRange && IsFacingTarget();
        }
        
        private bool IsFacingTarget()
        {
            var directionToTarget = (_target.position - transform.position).normalized;
            directionToTarget.y = 0f;
    
            var forward = transform.forward;
            forward.y = 0f;
    
            var angle = Vector3.Angle(forward.normalized, directionToTarget);
            return angle <= data.visionAngle;
        }

        #endregion
        
        private void OnDied()
        {
            onDied?.Invoke(data.rewards, transform.position);
            Destroy(gameObject, 0.2f);
        }

        private void OnDamageTaken(float amount, Vector3 knockback)
        {
            _movementHandler.ApplyKnockback(knockback, data.unitStats.knockbackResistance);

            var move = _attackHandler.ActiveMove;
            if (_attackHandler.IsAttacking && move != null && move.isInterruptable)
            {
                if (knockback.magnitude >= move.interruptThreshold)
                    _attackHandler.Interrupt();
            }
        }
    }
}
