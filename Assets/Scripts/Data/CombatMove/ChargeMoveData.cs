using Data.HitEffects;
using Data.Stats.Output;
using UnityEngine;

namespace Data.CombatMove
{
    /// <summary>
    /// Straight-line charge to a fixed point captured at cast time.
    /// Unlike Melee/Range, this move's real duration depends on travel distance,
    /// which isn't known when the asset is authored - so it can't resolve itself
    /// in a single Execute () call the way other moves do.
    /// 
    /// Execute() is intentionally near-empty. The actual movement, the
    /// trailing hitbox, and arrival detection are all driven by whatever
    /// owns this move (BossController) — it already has to know it's mid-charge
    /// to drive transform movement in its own Update(), so it reads this
    /// asset's config directly rather than going through Execute()'s
    /// generic (origin, direction, owner) contract, which has no room for
    /// "here's your destination, tell me when you arrive."
    ///
    /// No runtime state lives on this asset (it's shared data, not a
    /// per-boss instance) — arrival is reported by BossController into
    /// EnemyAttackHandler.ReportExternalCompletion(), and this move just
    /// reads that flag back out via IsExecutionComplete().
    /// </summary>
    [CreateAssetMenu(fileName = "New Charge Move", menuName = "Game/Combat Move/Charge Move")]
    public class ChargeMoveData : CombatMoveData
    {
        [Header("Charge Configuration")] 
        public float speed;
        public float arrivalThreshold = 0.3f;
        
        [Header("Trailing Hitbox")]
        public float hitBoxRadius;
        public DamageStats damageStats;
        public HitEffect[] hitEffects;
        public LayerMask obstacleDestroyMask;
        
        /// <summary>
        /// Fixed destination for this charge, captured once at cast time.
        /// BossController calls this when it commits to the move, then
        /// drives its own transform toward the result every frame.
        /// </summary>
        public Vector3 GetDestination(Vector3 targetPositionAtCastTime)
            => targetPositionAtCastTime;

        public override void Execute(Vector3 origin, Vector3 direction, GameObject owner)
        {
            // Deliberately empty - see class remarks. All real behavior is driven externally by
            // BossController, which already needs to own per-frame movement and can't delegate that
            // a single synchronous call.
        }

        public override bool IsExecutionComplete(float attackTime, bool externalCompletionFlag) 
            => externalCompletionFlag;
    }
}