using Data.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Handlers
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovementHandler : MonoBehaviour
    {
        // References
        private float _knockbackDecay;
        private NavMeshAgent _navAgent;
        
        // State
        private Vector3 _knockbackVelocity;

        private void Awake() => _navAgent = GetComponent<NavMeshAgent>();
        
        public bool IsStaggered()
        {
            if (_knockbackVelocity.sqrMagnitude <= 0.01f) return false;

            _knockbackVelocity = Vector3.MoveTowards(
                _knockbackVelocity,
                Vector3.zero,
                _knockbackDecay * Time.deltaTime
            );
            
            _navAgent.Move(_knockbackVelocity * Time.deltaTime);
            return true;
        }
        
        public void ApplyKnockback(Vector3 knockback, float resistance)
        {
            _knockbackVelocity = knockback / Mathf.Max(resistance, 0.01f);
            _navAgent.ResetPath();
        }
        
        public void MoveTo(Vector3 position) => _navAgent.SetDestination(position);
        
        public void Stop() => _navAgent.ResetPath();
        
        public void Init(UnitStats stats)
        {
            _navAgent.speed = stats.moveSpeed;
            _navAgent.angularSpeed = stats.rotationSpeed;
            _knockbackDecay = stats.knockbackDecay;
        }
    }
}