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

        // Config options
        public float shakeScale;
        public float shakeCap;

        // Local variables
        private float _lastHealth = -1f;

        // Linked modules
        [CanBeNull] private PlayerHealthController _mHealth;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnPlayerSpawn += OnPlayerSpawn;
            EventSystem._.OnPlayerDie += OnPlayerDie;
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
            EventSystem._.OnPlayerDie -= OnPlayerDie;
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

        private void OnPlayerDie(BsMap map, BsPlayer player)
        {
            if (player.Id != Master.Object.Id) return;
            CameraSystem._.AddShake(shakeCap);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            if (other.gameObject.CompareTag(Tags.PlayerTag)) return;
            var impactForce = other.TotalNormalImpulse();
            var impactDirection = ((Vector2)transform.localPosition - other.contacts[0].point).normalized;

            // Apply camera shove
            CameraSystem._.AddShove(Mathf.Min(PlayerHealthController.CalculateDamage(impactForce) *
                shakeScale, shakeCap) * impactDirection);
        }

        private void FixedUpdate()
        {
            // Apply camera shake after taking damage
            if (!_mHealth) return;
            var health = _mHealth.Health;
            if (_lastHealth < 0f)
            {
                _lastHealth = health;
                return;
            }
            var deltaHealth = health - _lastHealth;
            if (deltaHealth < 0f)
            {
                var shakeForce = Mathf.Min(-deltaHealth * shakeScale, shakeCap);
                CameraSystem._.AddShake(shakeForce);
            }
            _lastHealth = health;
        }
    }
}
