using Data.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Controllers
{
    public interface IEnemy
    { 
        UnityEvent<RewardStats, Vector3> OnDied { get; }
    }
}