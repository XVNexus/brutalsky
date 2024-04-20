using Brutalsky;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
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
        protected override void OnInit()
        {
            EventSystem._.OnPlayerRespawn += OnPlayerRespawn;
        }

        protected override void OnLink()
        {
            _mHealth = Master.GetSub<PlayerHealthController>("health");
        }

        // Event functions
        private void OnPlayerRespawn(BsMap map, BsPlayer player)
        {
            if (player.Id != Master.Object.Id) return;
            _lastHealth = -1f;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            if (other.gameObject.CompareTag(Tags.PlayerTag)) return;
            var impactForce = other.TotalNormalImpulse();
            var impactDirection = ((Vector2)transform.position - other.contacts[0].point).normalized;

            // Apply camera shake
            var shakeForce = Mathf.Min(PlayerHealthController.CalculateDamage(impactForce) * .15f, 15f);
            CameraSystem._.Shove(shakeForce * impactDirection);
        }

        private void FixedUpdate()
        {
            // Apply camera shake after taking damage
            if (!_mHealth) return;
            var health = _mHealth.health;
            if (_lastHealth < 0f)
            {
                _lastHealth = health;
                return;
            }
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
