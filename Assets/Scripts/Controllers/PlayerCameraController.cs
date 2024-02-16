using System.Linq;
using Brutalsky;
using Core;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerCameraController : MonoBehaviour
    {
        // Variables
        private float lastHealth = -1f;

        // References
        public PlayerController cPlayerController;

        // Events
        private void Start()
        {
            cPlayerController = GetComponent<PlayerController>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            var impactForce = other.contacts.Sum(contact => contact.normalImpulse);
            var impactDirection = ((Vector2)transform.position - other.contacts[0].point).normalized;

            // Apply camera shake
            var shakeForce = Mathf.Min(Mathf.Pow(BsPlayer.CalculateDamage(impactForce), 1.5f) * .01f, 5f);
            if (other.gameObject.CompareTag("Player"))
            {
                CameraSystem.current.Shake(shakeForce);
            }
            else
            {
                CameraSystem.current.Shove(shakeForce * impactDirection);
            }
        }

        // Updates
        private void FixedUpdate()
        {
            // Apply camera shake after taking damage
            var health = cPlayerController.health;
            var deltaHealth = health - lastHealth;
            if (deltaHealth < 0f)
            {
                var shakeForce = Mathf.Min(-deltaHealth * .05f, 5f);
                CameraSystem.current.Shake(shakeForce);
            }
            lastHealth = health;
        }
    }
}
