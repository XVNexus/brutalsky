using Brutalsky;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils;
using Utils.Constants;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerCameraController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "camera";
        public override bool IsUnused => false;

        // Local variables
        private float _lastHealth = -1f;

        // Linked modules
        [CanBeNull] private PlayerHealthController _mHealth;

        // Init functions
        protected override void OnLink()
        {
            _mHealth = Master.GetSub<PlayerHealthController>("health");
        }

        // Event functions
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            if (other.gameObject.CompareTag(Tags.Player)) return;
            var impactForce = other.TotalNormalImpulse();
            var impactDirection = ((Vector2)transform.position - other.contacts[0].point).normalized;

            // Apply camera shake
            var shakeForce = Mathf.Min(BsUtils.CalculateDamage(impactForce) * .15f, 15f);
            CameraSystem._.Shove(shakeForce * impactDirection);
        }

        private void FixedUpdate()
        {
            // Apply camera shake after taking damage
            if (!_mHealth) return;
            var health = _mHealth.health;
            var deltaHealth = health - _lastHealth;
            if (deltaHealth < 0f)
            {
                var shakeForce = Mathf.Min(-deltaHealth * .15f, 15f);
                CameraSystem._.Shake(shakeForce);
            }
            if (health == 0f && _lastHealth > 0f)
            {
                CameraSystem._.Shake(10f);
            }
            _lastHealth = health;
        }
    }
}
