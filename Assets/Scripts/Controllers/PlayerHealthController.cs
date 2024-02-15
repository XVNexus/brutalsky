using System.Linq;
using Core;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerHealthController : MonoBehaviour
    {
        // Variables
        private float lastSpeed;

        // References
        private PlayerController cPlayerController;
        private Rigidbody2D cRigidbody2D;

        // Events
        private void Start()
        {
            cPlayerController = GetComponent<PlayerController>();
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other) => OnCollision(other);

        private void OnCollisionStay2D(Collision2D other) => OnCollision(other);

        private void OnCollision(Collision2D other)
        {
            if (!cPlayerController.alive) return;

            // Get collision info
            var impactForce = other.contacts.Sum(contact => contact.normalImpulse);
            if (impactForce < 25f) return;
            var impactSpeed = lastSpeed;

            // Apply damage to player
            var damageApplied = MathfExt.TMP(impactForce, 25f, .5f, 1.5f);
            var damageMultiplier = Mathf.Min(1f / (impactSpeed * .2f), 1f);
            var damage = damageApplied * damageMultiplier;
            cPlayerController.health = Mathf.Max(cPlayerController.health - damage, 0f);
        }

        // Updates
        private void FixedUpdate()
        {
            // Save the current speed for future reference
            lastSpeed = cRigidbody2D.velocity.magnitude;
        }
    }
}
