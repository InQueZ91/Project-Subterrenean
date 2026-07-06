using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Beam
{
    /// <summary>
    /// Pure logic — solves a full beam path (including bounces) in one shot.
    /// No MonoBehaviour. No Update loop. Mirrors Projectile's ricochet math,
    /// just resolved instantly across multiple segments instead of over frames.
    /// </summary>
    public static class BeamSolver
    {
        private const float RicochetNudge = 0.01f;
        private const float RicochetMinDot = -0.1f;
        private const int SafetyIterationCap = 32; // hard stop even if maxBounces is misconfigured

        public static List<BeamSegment> Solve(
            Vector3 origin,
            Vector3 direction,
            float maxRange,
            int maxBounces,
            LayerMask hitMask)
        {
            var segments = new List<BeamSegment>();

            var currentOrigin = origin;
            var currentDirection = direction.normalized;
            var remainingRange = maxRange;
            var bouncesLeft = maxBounces;
            var iterations = 0;

            while (remainingRange > 0f && iterations < SafetyIterationCap)
            {
                iterations++;

                if (!Physics.Raycast(currentOrigin, currentDirection, out var hit, remainingRange, hitMask))
                {
                    // Nothing hit – beam travels its full remaining range and stops
                    var end = currentOrigin + currentDirection * remainingRange;
                    segments.Add(new BeamSegment(currentOrigin, end, null, null, currentDirection));
                    break;
                }
                
                segments.Add(new BeamSegment(currentOrigin, hit.point, hit.collider, hit.normal, currentDirection));
                remainingRange -= hit.distance;

                if (bouncesLeft <= 0) break;

                var flatNormal = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
                if (flatNormal == Vector3.zero) break; // flat floor/ceiling - can't bounce on XZ

                var dot = Vector3.Dot(currentDirection, flatNormal);
                if (dot >= RicochetMinDot) break; // too head-on to bounce

                currentDirection = Vector3.Reflect(currentDirection, flatNormal).normalized;
                currentOrigin = hit.point + hit.normal * RicochetNudge;
                bouncesLeft--;
            }
            
            return segments;
        }
    }
}