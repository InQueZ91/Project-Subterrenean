using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Items
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshObstacle))]
    public class BarricadeInstance : MonoBehaviour
    {
        private Unit _unit;
        private NavMeshObstacle _navMeshObstacle;

        public void Init(float health)
        {
            _unit = GetComponent<Unit>();
            _unit.Init(health);
            _unit.onDied.AddListener(OnDied);
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            _navMeshObstacle.carving = true;
        }

        private void OnDestroy()
        {
            _unit.onDied.RemoveAllListeners();
        }

        private void OnDied()
        {
            Destroy(gameObject);
        }
    }
}