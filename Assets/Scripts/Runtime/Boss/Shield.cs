using System;
using System.Collections;
using Data.Stats;
using UnityEngine;

namespace Runtime.Boss
{
    /// <summary>
    /// Purely reactive — BossController decides when the shield is up or down
    /// (derived from its own attack state, per design: shield is default-on,
    /// melee moves lower it during their windup/execute/recovery). This
    /// component just presents that state: toggles its own collider and
    /// plays an animation trigger. No shield logic lives here.
    ///
    /// Collider should share the same tag/layer as map geometry (walls,
    /// ground) so Projectile's existing pierce/bounce handling treats it
    /// identically to environment collision — no new projectile-side code
    /// needed for the deflection behavior itself.
    /// </summary>
    [RequireComponent(typeof(Unit))]
    public class Shield : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Collider shieldCollider;
        [SerializeField] private Animator animator;
        
        [Header("Animation Triggers")]
        [SerializeField] private string raiseTrigger = "Raise";
        [SerializeField] private string lowerTrigger = "Lower";
        
        [Header("Configuration")]
        [SerializeField] private UnitStats unitStats;
        [SerializeField] private float regenerateDuration = 5f;

        private Unit _unit;
        private bool _isRegenerating;
        
        private void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        private void Start()
        {
            _unit.Init(unitStats);
            _unit.onDied.AddListener(OnShieldDestroyed);
        }

        private void OnDisable()
        {
            _unit.onDied.RemoveListener(OnShieldDestroyed);
        }

        public void SetRaised(bool raised)
        {
            if (raised && _isRegenerating)
            {
                // CancelRegeneration();
                return;
            }
 
            SetColliderEnabled(raised);
            PlayAnimation(raised ? raiseTrigger : lowerTrigger);
        }

        #region private helper

        private void SetColliderEnabled(bool enable)
        {
            if (shieldCollider != null)
                shieldCollider.enabled = enable;
        }
 
        private void PlayAnimation(string trigger)
        {
            if (animator != null)
                animator.Play(trigger);
        }
 
        private void CancelRegeneration()
        {
            StopAllCoroutines();
            _isRegenerating = false;
            RestoreShield();
        }
 
        private void RestoreShield()
        {
            _unit.Heal(unitStats.maxHealth);
            SetColliderEnabled(true);
            PlayAnimation(raiseTrigger);
        }

        #endregion

        private void OnShieldDestroyed()
        {
            Debug.Log("[Shield] Shield destroyed — beginning regeneration.");
            SetColliderEnabled(false);
            PlayAnimation(lowerTrigger);
            StartCoroutine(RegenerateCoroutine());
        }
 
        private IEnumerator RegenerateCoroutine()
        {
            _isRegenerating = true;
            yield return new WaitForSeconds(regenerateDuration);
            _isRegenerating = false;
            RestoreShield();
        }
    }
}