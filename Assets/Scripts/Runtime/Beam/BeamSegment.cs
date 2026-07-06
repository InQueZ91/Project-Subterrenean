using UnityEngine;

namespace Runtime.Beam
{
    /// <summary>
    /// One straight-line piece of a solved beam path.
    /// HitCollider/HitNormal are null when the beam reaches max range
    /// without hitting anything (the final, unobstructed segment).
    /// </summary>
    public readonly struct BeamSegment
    {
        public readonly Vector3 start;
        public readonly Vector3 end;
        public readonly Collider hitCollider;
        public readonly Vector3? hitNormal;
        public readonly Vector3 direction; // travel direction for this specific segment
        
        public BeamSegment(Vector3 start, Vector3 end, Collider hitCollider, Vector3? hitNormal, Vector3 direction)
        {
            this.start = start;
            this.end = end;
            this.hitCollider = hitCollider;
            this.hitNormal = hitNormal;
            this.direction = direction;
        }
    }
}