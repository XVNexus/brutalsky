using Brutalsky;
using UnityEngine;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerHealthController : MonoBehaviour
    {
        // Variables
        private float _lastSpeed;

        // References
        private PlayerController _cPlayerController;
        private Rigidbody2D _cRigidbody2D;

        // Events
        private void Start()
        {
            _cPlayerController = GetComponent<PlayerController>();
            _cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other) => OnCollision(other);

        private void OnCollisionStay2D(Collision2D other) => OnCollision(other);

        private void OnCollision(Collision2D other)
        {
            if (!_cPlayerController.alive) return;

            // Get collision info
            var impactForce = other.TotalNormalImpulse() * (other.gameObject.CompareTag(PlayerController.Tag) ? 2f : 1f);
            if (impactForce < 25f) return;
            var impactSpeed = _lastSpeed;

            // Apply damage to player
            var damageApplied = BsPlayer.CalculateDamage(impactForce);
            var damageMultiplier = Mathf.Min(1f / (impactSpeed * .2f), 1f);
            _cPlayerController.Hurt(damageApplied * damageMultiplier);
        }

        // Updates
        private void FixedUpdate()
        {
            // Save the current speed for future reference
            _lastSpeed = _cRigidbody2D.velocity.magnitude;
        }
    }
}
