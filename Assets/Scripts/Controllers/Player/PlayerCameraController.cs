using Brutalsky;
using Core;
using UnityEngine;
using Utils;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerCameraController : SubControllerBase<PlayerController, BsPlayer>
    {
        public override bool IsUnused => false;

        private float _lastHealth = -1f;

        private PlayerController _cPlayerController;

        protected override void OnInit()
        {
            _cPlayerController = GetComponent<PlayerController>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            if (other.gameObject.CompareTag(Tags.Player)) return;
            var impactForce = other.TotalNormalImpulse();
            var impactDirection = ((Vector2)transform.position - other.contacts[0].point).normalized;

            // Apply camera shake
            var shakeForce = Mathf.Min(BsPlayer.CalculateDamage(impactForce) * .15f, 15f);
            CameraSystem._.Shove(shakeForce * impactDirection);
        }

        private void FixedUpdate()
        {
            // Apply camera shake after taking damage
            var health = _cPlayerController.health;
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
