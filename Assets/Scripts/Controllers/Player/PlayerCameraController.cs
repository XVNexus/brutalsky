using Brutalsky;
using Brutalsky.Object;
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
            EventSystem._.OnPlayerSpawn += OnPlayerSpawn;
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
        }

        protected override void OnLink()
        {
            _mHealth = Master.GetSub<PlayerHealthController>("health");
        }

        // Event functions
        private void OnPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visible)
        {
            if (player.Id != Master.Object.Id) return;
            _lastHealth = -1f;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            if (other.gameObject.CompareTag(Tags.PlayerTag)) return;
            var impactForce = other.TotalNormalImpulse();
            var impactDirection = ((Vector2)transform.localPosition - other.contacts[0].point).normalized;

            // Apply camera shake
            var shakeForce = Mathf.Min(PlayerHealthController.CalculateDamage(impactForce) * .05f, 5f);
            CameraSystem._.AddShove(shakeForce * impactDirection);
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
                var shakeForce = Mathf.Min(-deltaHealth * .05f, 5f);
                CameraSystem._.AddShake(shakeForce);
            }
            if (health == 0f && _lastHealth > 0f)
            {
                CameraSystem._.AddShake(5f);
            }
            _lastHealth = health;
        }
    }
}
