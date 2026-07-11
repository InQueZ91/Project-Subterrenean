using Data.Output;
using Runtime.Spawners;
using UnityEngine;

namespace Data.CombatMove
{
    [CreateAssetMenu(fileName = "New Ranged Move", menuName = "Game/Combat Move/Ranged Move")]
    public class RangeMoveData : CombatMoveData
    {
        [Header("Configuration")]
        public ProjectileOutputData projectileOutput;

        public override void Execute(Vector3 origin, Vector3 direction, GameObject owner) 
            => ProjectileSpawner.Instance.Spawn(projectileOutput, projectileOutput.stats, origin, direction, owner);
    }
}