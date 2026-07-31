using Data;
using Runtime;
using Runtime.Handlers.Machine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Controllers
{
    [RequireComponent(typeof(NavMeshObstacle))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(StorageHandler))]
    public class MachineController : MonoBehaviour
    {
        [SerializeField] private StartingItemData[] startingItems;

        [Header("Events")] 
        public UnityEvent onPlayerEntered;
        public UnityEvent onPlayerExited;
        
        private NavMeshObstacle _obstacle;
        private StorageHandler _storageHandler;

        private bool _isPlayerInside;
        
        private void Awake()
        {
            _storageHandler = GetComponent<StorageHandler>();
            _obstacle = GetComponent<NavMeshObstacle>();
        }

        private void Start()
        {
            _storageHandler.Init(startingItems);
            _obstacle.carving = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var projectile = other.GetComponent<Projectile>();
            if (projectile != null && _isPlayerInside)
            {
                Destroy(projectile.gameObject);
            }
            
            if (!other.CompareTag("Player")) return;
            
            _isPlayerInside = true;
            _obstacle.carving = true;
            onPlayerEntered?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            _isPlayerInside = false;
            _obstacle.carving = false;
            onPlayerExited?.Invoke();
        }
    }
}