using System.Linq;
using Brutalsky;
using Brutalsky.Object;
using Core;
using UnityEngine;

namespace Controllers.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        // Variables
        private float lastHealth = -1f;

        // References
        private PlayerController cPlayerController;

        // Events
        private void Start()
        {
            cPlayerController = GetComponent<PlayerController>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            if (other.gameObject.CompareTag("Player")) return;
            var impactForce = other.contacts.Sum(contact => contact.normalImpulse);
            var impactDirection = ((Vector2)transform.position - other.contacts[0].point).normalized;

            // Apply camera shake
            var shakeForce = Mathf.Min(BsPlayer.CalculateDamage(impactForce) * .03f, 3f);
            CameraSystem.current.Shove(shakeForce * impactDirection);
        }

        // Updates
        private void FixedUpdate()
        {
            // Apply camera shake after taking damage
            var health = cPlayerController.health;
            var deltaHealth = health - lastHealth;
            if (deltaHealth < 0f)
            {
                var shakeForce = Mathf.Min(-deltaHealth * .03f, 3f);
                CameraSystem.current.Shake(shakeForce);
            }
            if (health == 0f && lastHealth > 0f)
            {
                CameraSystem.current.Shake(2f);
            }
            lastHealth = health;
        }
    }
}
