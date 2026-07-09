using Data.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Items
{
    [RequireComponent(typeof(Unit))]
    [RequireComponent(typeof(NavMeshObstacle))]
    public class BarricadeInstance : MonoBehaviour
    {
        private Unit _unit;
        private NavMeshObstacle _navMeshObstacle;

        public void Init(UnitStats unitStats)
        {
            _unit = GetComponent<Unit>();
            _unit.Init(unitStats);
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