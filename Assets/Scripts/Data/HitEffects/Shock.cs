using System.Collections.Generic;
using System.Linq;
using Runtime;
using UnityEngine;

namespace Data.HitEffects
{
    [CreateAssetMenu(fileName = "New Shock", menuName = "Game/Hit Effects/Shock")]
    public class Shock : HitEffect
    {
        [Header("Properties")]
        [SerializeField] private float baseDamage = 1f;
        [SerializeField] private float baseMagnitude = 2f;
        [SerializeField] private int numberOfChains = 1;
        [SerializeField] private float falloff = 0.5f;
        [SerializeField] private float radius = 2f;
        
        [Header("Collision")]
        [Tooltip("Which layers to check for hits")]
        [SerializeField] private LayerMask hitLayers;
        
        public override void Apply(HitContext ctx)
        {
            var originalDamageable = ctx.victim.GetComponent<IDamageable>();
            originalDamageable?.TakeDamage(baseDamage, Vector3.down * baseMagnitude);
            
            var visited = new HashSet<GameObject>() { ctx.victim };
            var headOfChain = ctx.victim;
            var damage = baseDamage; 
            var magnitude = baseMagnitude;
            var chainNumber = numberOfChains;
            
            while (chainNumber > 0)
            {
                var hitCount = Physics.OverlapSphereNonAlloc(headOfChain.transform.position, radius, OverlapBuffer, hitLayers);

                var nextVictim = FindClosestValidTarget(headOfChain, ctx.owner, visited, hitCount);
                
                if (nextVictim == null) break;
                
                var damageable = nextVictim.GetComponent<IDamageable>();
                if (damageable == null) break;
                
                var dist = (nextVictim.position - headOfChain.transform.position).magnitude;
                var normalizedDist = Mathf.Clamp01(dist / radius);
                var multiplier = Mathf.Pow(1f - normalizedDist, falloff);
                
                damage *= multiplier;
                magnitude *= multiplier;
                damageable.TakeDamage(damage, Vector3.down * magnitude);
                
                // Apply shock
                
                visited.Add(nextVictim.gameObject);
                headOfChain = nextVictim.gameObject;
                chainNumber--;
            }
        }
        
        private static readonly Collider[] OverlapBuffer = new Collider[32];
        
        private Transform FindClosestValidTarget(
            GameObject chainHead,
            GameObject owner,
            HashSet<GameObject> visited,
            int hitCount)
        {
            return OverlapBuffer
                .Take(hitCount)
                .Where(c => c != null
                            && !visited.Contains(c.gameObject)
                            && (c.gameObject != owner || isFriendlyFire))
                .OrderBy(c => (c.transform.position - chainHead.transform.position).sqrMagnitude)
                .Select(c => c.transform)
                .FirstOrDefault();
        }
    }
}