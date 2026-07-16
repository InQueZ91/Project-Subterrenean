using System.Collections.Generic;
using Data.Bosses;
using Data.CombatMove;
using Data.Stats;
using Runtime;
using Runtime.Boss;
using Runtime.Handlers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Controllers
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class BossController : MonoBehaviour
    {
        [SerializeField] private BossData data;
        [SerializeField] private Transform firingPoint;
        [SerializeField] private Shield shield;
        [SerializeField] private Collider bossHitbox; // boss's own hittable collider, disabled during charge
        [SerializeField] private ChargeHitbox chargeHitbox;

        // References
        private Transform _target;
        private Unit _unit;
        private EnemyAttackHandler _attackHandler;
        private EnemyMovementHandler _movementHandler;

        // Knockback (same convention as EnemyController)
        private Vector3 _knockbackVelocity;

        // Phase
        private enum Phase
        {
            Normal,
            Enraged
        }

        private Phase _phase = Phase.Normal;
        private float _currentHealthPercent = 1f;

        // Super slam sequencing
        private readonly HashSet<float> _firedSuperSlamThresholds = new();
        private int _superSlamStageIndex = -1; // -1 = not currently in a super slam sequence

        // Charge driving state
        private bool _isCharging;
        private Vector3 _chargeDestination;

        [Header("Events")] 
        public UnityEvent<RewardStats, Vector3> onDied;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _movementHandler = GetComponent<EnemyMovementHandler>();
            _attackHandler = new EnemyAttackHandler();
            
            _attackHandler.OnWindupStarted += OnMoveWindupStarted;
            _attackHandler.OnRecoveryStarted += OnMoveRecoveryStarted;
            _attackHandler.OnAttackEnded += OnMoveAttackEnded;
        }

        private void Start()
        {
            _unit.Init(data.unitStats);
            _unit.onHealthChanged.AddListener(OnHealthChanged);
            _unit.onDamageTaken.AddListener(OnDamageTaken);
            _unit.onDied.AddListener(OnDied);
            
            _movementHandler.Init(data.unitStats);
            
            var player = GameObject.FindWithTag("Player");
            if (player != null) _target = player.transform;
            
            shield?.SetRaised(true);
            
            chargeHitbox?.Init(data.chargeMove, gameObject);
            chargeHitbox?.Deactivate();
        }

        private void OnDisable()
        {
            _unit.onHealthChanged.RemoveListener(OnHealthChanged);
            _unit.onDamageTaken.RemoveListener(OnDamageTaken);
            _unit.onDied.RemoveListener(OnDied);

            _attackHandler.OnWindupStarted -= OnMoveWindupStarted;
            _attackHandler.OnRecoveryStarted -= OnMoveRecoveryStarted;
            _attackHandler.OnAttackEnded -= OnMoveAttackEnded;
        }

        private void Update()
        {
            if (_target == null) return;

            if (_isCharging) { TickCharge(); return; }
            
            if (_movementHandler.IsStaggered()) return;
            
            if (_attackHandler.IsAttacking)
            {
                var direction = (_target.position - transform.position).normalized;
                _attackHandler.Tick(Time.deltaTime, firingPoint.position, direction, gameObject);
                return;
            }

            // Super slam sequence takes priority over normal move selection -
            // once triggerred, see all 3 stages through before picking anything else.
            if (_superSlamStageIndex >= 0)
            {
                BeginNextSuperSlamStage();
                return;
            }

            HandleMoveSelection();
        }

        #region Move Selection

        private void HandleMoveSelection()
        {
            var selectedMove = _phase == Phase.Normal ? GetNormalPhaseMove() : GetEnragedPhaseMove();

            if (selectedMove == null)
            {
                _movementHandler.MoveTo(_target.position);
                return;
            }

            if (selectedMove is ChargeMoveData chargeMove)
            {
                BeginCharge(chargeMove);
                return;
            }

            _movementHandler.Stop();
            _attackHandler.BeginAttack(selectedMove);
        }

        private CombatMoveData GetNormalPhaseMove()
        {
            return IsInRange(data.normalMove) ? data.normalMove : null;
        }

        private CombatMoveData GetEnragedPhaseMove()
        {
            if (IsInRange(data.normalMove))
                return data.normalMove;

            return GetRandomRangedOrChargeMove();
        }

        private CombatMoveData GetRandomRangedOrChargeMove()
        {
            var candidates = new List<CombatMoveData> { data.chargeMove, data.rangeMove }
                .FindAll(IsInRange);

            return candidates.Count == 0 ? null : candidates[Random.Range(0, candidates.Count)];
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

        private bool IsInRange(CombatMoveData move)
        {
            if (move == null) return false;

            var distanceToTarget = Vector3.Distance(transform.position, _target.position);
            return distanceToTarget <= move.activeRange && IsFacingTarget();
        }

        #endregion
        
        #region Charge Tackle

        private void BeginCharge(ChargeMoveData chargeMove)
        {
            _isCharging = true;
            _chargeDestination = chargeMove.GetDestination(_target.position);
            _movementHandler.Stop();

            // if (bossHitbox != null) bossHitbox.enabled = false;
            shield?.SetRaised(false);

            chargeHitbox?.Activate();

            _attackHandler.BeginAttack(chargeMove);
        }

        private void TickCharge()
        {
            var chargeMove = _attackHandler.ActiveMove as ChargeMoveData;
            if (chargeMove == null)
            {
                EndCharge();
                return;
            }

            var toDestination = _chargeDestination - transform.position;
            toDestination.y = 0f;
            var arrived = toDestination.magnitude <= chargeMove.arrivalThreshold;
            if (!arrived)
            {
                var moveStep = toDestination.normalized * (chargeMove.speed * Time.deltaTime);
                transform.position += moveStep;
                if (toDestination.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(toDestination.normalized);
            }

            _attackHandler.ReportExternalCompletion(arrived);
            _attackHandler.Tick(Time.deltaTime, transform.position, transform.forward, gameObject);

            if (arrived) EndCharge();
        }

        private void EndCharge()
        {
            _isCharging = false;
            chargeHitbox?.Deactivate();
            shield?.SetRaised(true);
        }

        #endregion

        #region Super Slam

        private void TriggerSuperSlam()
        {
            // Interrupt whatever's currently happening - super slam preempts.
            if (_isCharging) EndCharge();
            if (_attackHandler.IsAttacking) _attackHandler.Interrupt();

            _superSlamStageIndex = 0;
            
            Debug.Log("Super slam triggered!");
        }

        private void BeginNextSuperSlamStage()
        {
            if (data.superSlamStages == null || _superSlamStageIndex >= data.superSlamStages.Length)
            {
                _superSlamStageIndex = -1;
                return;
            }

            var stage = data.superSlamStages[_superSlamStageIndex];
            if (stage == null)
            {
                _superSlamStageIndex = -1;
                return;
            }

            _movementHandler.Stop();
            _attackHandler.BeginAttack(stage);
        }

        #endregion
                
        #region Event reactions

        private void OnMoveWindupStarted(CombatMoveData move)
        {
            // Melee moves (slam, super slam stages) lower the shield for
            // their windup/execute/recovery — shield state is derived from
            // "am I mid-melee", not tracked independently.
            if (move is MeleeMoveData)
                shield?.SetRaised(false);
        }
 
        private void OnMoveRecoveryStarted(CombatMoveData move) { /* no-op for now */ }
 
        private void OnMoveAttackEnded(CombatMoveData move)
        {
            // If we just finished a super slam stage, advance to the next
            // one (or exit the sequence) on the following Update.
            if (_superSlamStageIndex >= 0)
            {
                _superSlamStageIndex++;
                if (data.superSlamStages == null || _superSlamStageIndex >= data.superSlamStages.Length)
                {
                    _superSlamStageIndex = -1;
                }
            }
 
            // Only raise the shield once the whole melee action is truly
            // over — mid-super-slam-sequence (stage 1 -> stage 2 -> stage 3)
            // the shield should stay down throughout, not flicker up
            // between individual hits.
            var stillInSuperSlamSequence = _superSlamStageIndex >= 0;
            if (move is MeleeMoveData && !stillInSuperSlamSequence)
                shield?.SetRaised(true);
        }
 
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
        
        private void OnHealthChanged(float healthPercent)
        {
            _currentHealthPercent = healthPercent;

            if (_phase == Phase.Normal && _currentHealthPercent <= data.enrageHealthPercent)
            {
                _phase = Phase.Enraged;
            }

            foreach (var threshold in data.ultimateHealthThresholds)
            {
                if (_currentHealthPercent <= threshold && _firedSuperSlamThresholds.Add(threshold))
                {
                    TriggerSuperSlam();
                }
            }
        }

        #endregion
    }
}