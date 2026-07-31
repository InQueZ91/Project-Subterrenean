using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MiniMapUI : MonoBehaviour
    {
        // Inspector References
        [Header("Core References")]
        [Tooltip("The RectTransform of the minimap panel.")]
        public RectTransform minimapRect;
 
        [Tooltip("UI Image dot that follows the player.")]
        public RectTransform playerIcon;
 
        [Tooltip("UI Image dot for the platform / spawn point (static).")]
        public RectTransform platformIcon;
 
        [Tooltip("Prefab for an enemy blip (small colored Image).")]
        public GameObject enemyIconPrefab;
        
        // World space inputs
        [Header("World Transforms")]
        [Tooltip("Assign the player's Transform at runtime or in Inspector.")]
        public Transform playerTransform;
 
        [Tooltip("Assign the platform/spawn Transform at runtime or in Inspector.")]
        public Transform platformTransform;
 
        [Header("Map Scale")]
        [Tooltip("Diameter of the game world in world-units that the minimap covers.")]
        public float worldSize = 50f;
        
        // Optional rotation
        [Header("Rotation")]
        [Tooltip("If true the map rotates so the player always faces 'up'.")]
        public bool rotateWithPlayer = false;
        
        // Runtime State
        // Maps each enemy Transform → its minimap blip RectTransform
        private readonly Dictionary<Transform, RectTransform> _enemyBlips
            = new Dictionary<Transform, RectTransform>();
        
        // Unity Lifecycle
        private void Start()
        {
            if (platformTransform != null)
                PlaceIcon(platformIcon, platformTransform.position);
        }
        
        private void LateUpdate()
        {
            if (playerTransform == null) return;
 
            // 1. Move player icon
            PlaceIcon(playerIcon, playerTransform.position);
 
            // 2. Optionally rotate the whole map so the player faces up
            if (rotateWithPlayer)
            {
                var angle = playerTransform.eulerAngles.y;
                minimapRect.localRotation = Quaternion.Euler(0f, 0f, angle);
                // Counter-rotate icons so they stay upright
                playerIcon.localRotation   = Quaternion.Euler(0f, 0f, -angle);
                platformIcon.localRotation = Quaternion.Euler(0f, 0f, -angle);
            }
            
            // 3. Update every enemy blip
            foreach (var kvp in _enemyBlips)
            {
                if (kvp.Key == null) continue;          // enemy was destroyed without unregister
                PlaceIcon(kvp.Value, kvp.Key.position);
 
                if (rotateWithPlayer)
                    kvp.Value.localRotation = Quaternion.Euler(0f, 0f,
                        -playerTransform.eulerAngles.y);
            }
        }
        
        // Public API
        public void RegisterEnemy(Transform enemy)
        {
            if (enemy == null || _enemyBlips.ContainsKey(enemy)) return;
 
            var blipGo = Instantiate(enemyIconPrefab, minimapRect);
            var blip = blipGo.GetComponent<RectTransform>();
            _enemyBlips[enemy] = blip;
        }
        
        public void UnregisterEnemy(Transform enemy)
        {
            if (enemy == null || !_enemyBlips.TryGetValue(enemy, out RectTransform blip)) return;
 
            if (blip != null) Destroy(blip.gameObject);
            _enemyBlips.Remove(enemy);
        }
 
        public void RegisterEnemies(IEnumerable<Transform> enemies)
        {
            foreach (var e in enemies) RegisterEnemy(e);
        }
        
        // Private helpers
        /// <summary>
        /// Converts a world-space XZ position into an anchored minimap position
        /// and applies it to the given icon's RectTransform.
        ///
        /// Coordinate mapping:
        ///   World X  →  UI anchoredPosition.x
        ///   World Z  →  UI anchoredPosition.y
        ///   (Y axis is height and is ignored for a top-down minimap)
        /// </summary>
        private void PlaceIcon(RectTransform icon, Vector3 worldPos)
        {
            if (icon == null) return;
 
            // Normalize world position into [-0.5 .. +0.5] range
            var normX = Mathf.Clamp(worldPos.x / worldSize, -0.5f, 0.5f);
            var normZ = Mathf.Clamp(worldPos.z / worldSize, -0.5f, 0.5f);
 
            // Scale to the minimap panel's pixel dimensions
            var mapW = minimapRect.rect.width;
            var mapH = minimapRect.rect.height;
 
            icon.anchoredPosition = new Vector2(normX * mapW, normZ * mapH);
        }
    }
}