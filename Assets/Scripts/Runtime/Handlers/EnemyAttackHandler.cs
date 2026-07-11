using System;
using Data.CombatMove;
using UnityEngine;

namespace Runtime.Handlers
{
    /// <summary>
    /// Owns a single enemy's attack-cycle state machine: windup => hit window => recovery.
    /// Driven off one accumulating clock rather than per-state timers.
    /// Plain C# class, composed by EnemyController (or later, Boss Controller) -
    /// no MonoBehaviour lifecycle of its own. Caller drives it via Tick() each frame and owns
    ///  the actual NavMeshAgent/movement/targeting decisions.
    /// </summary>
    public class EnemyAttackHandler
    {
        public enum AttackState
        {
            WindingUp,
            HitWindowActive,
            Recovering
        }
        
        public bool IsAttacking { get; private set; }
        public CombatMoveData ActiveMove { get; private set; }

        private float _attackTime;
        private AttackState _attackState;
        private bool _hasExecutedThisWindow;

        public event Action<CombatMoveData> OnWindupStarted;
        public event Action<CombatMoveData> OnHitWindowStarted;
        public event Action<CombatMoveData> OnRecoveryStarted;
        public event Action<CombatMoveData> OnAttackEnded;

        public void BeginAttack(CombatMoveData move)
        {
            IsAttacking = true;
            ActiveMove = move;
            _attackTime = 0f;
            _hasExecutedThisWindow = false;

            _attackState = AttackState.WindingUp;
            OnWindupStarted?.Invoke(move);
        }

        /// <summary>
        /// Advances the attack clock. Origin/Direction/Owner are only needed
        /// at the moment the hit window opens, so they're passed in rather
        /// than cached - direction resolves against wherever the target
        /// actually is when the hit fires, not where it was at commit time.
        /// </summary>
        public void Tick(float deltaTime, Vector3 origin, Vector3 direction, GameObject owner)
        {
            if (!IsAttacking || ActiveMove == null) return;

            var move = ActiveMove;
            _attackTime += deltaTime;

            var computedState = ComputeState(move, _attackTime);

            if (computedState != _attackState)
            {
                _attackState = computedState;
                switch (_attackState)
                {
                    case AttackState.HitWindowActive:
                        OnHitWindowStarted?.Invoke(move);
                        break;
                    case AttackState.Recovering:
                        OnRecoveryStarted?.Invoke(move);
                        break;
                }
            }

            if (_attackState == AttackState.HitWindowActive && !_hasExecutedThisWindow)
            {
                move.Execute(origin, direction, owner);
                _hasExecutedThisWindow = true;
            }

            if (_attackTime >= move.cooldown)
            {
                EndAttack();
            }
        }

        /// <summary>
        /// Cancels the in-progress attack without executing it (if it hasn't fired yet)
        /// or without further effect (if it already has).
        /// Caller is responsible for deciding whether the active move allows this -
        /// this method doesn't check isInterruptable/interruptThreshold itself.
        /// </summary>
        public void Interrupt()
        {
            if (!IsAttacking) return;
            var move = ActiveMove;
            ResetState();
            OnAttackEnded?.Invoke(move);
        }

        private void EndAttack()
        {
            var move = ActiveMove;
            ResetState();
            OnAttackEnded?.Invoke(move);
        }

        private void ResetState()
        {
            IsAttacking = false;
            ActiveMove = null;
            _attackTime = 0f;
        }

        private static AttackState ComputeState(CombatMoveData move, float attackTime)
        {
            if (attackTime < move.hitWindowStart) return AttackState.WindingUp;
            if (attackTime <= move.hitWindowEnd) return AttackState.HitWindowActive;
            return AttackState.Recovering;
        }
    }
}