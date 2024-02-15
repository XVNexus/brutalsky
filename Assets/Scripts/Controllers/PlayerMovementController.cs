using System;
using System.Linq;
using UnityEngine;

namespace Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        // Variables
        public float moveForce = 25f;
        private bool lastBoostInput;

        // References
        private PlayerController cPlayerController;
        private Rigidbody2D cRigidbody2D;
    
        // Events
        private void Start()
        {
            cPlayerController = GetComponent<PlayerController>();
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            // Get collision info
            var impactForce = other.contacts.Sum(contact => contact.normalImpulse);
            
            // Reduce velocity based on collision force
            var velocityFactor = Mathf.Min(1f / (impactForce * .1f), 1f);
            cRigidbody2D.velocity *= velocityFactor;
        }

        // Updates
        private void FixedUpdate()
        {
            // Apply directional movement
            var movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            cRigidbody2D.AddForce(movementInput * moveForce);

            // Apply boost movement
            var boostInput = Input.GetButton("Jump") && cPlayerController.boostCooldown == 0f;
            if (boostInput)
            {
                cPlayerController.boostCharge = Mathf.Min(cPlayerController.boostCharge + Time.fixedDeltaTime, 3f);
            }
            else if (lastBoostInput)
            {
                var boost = Mathf.Pow(cPlayerController.boostCharge, 2f) + 2f;
                cPlayerController.boostCooldown = boost;
                cPlayerController.boostCharge = 0f;
                cRigidbody2D.velocity *= boost;
            }
            if (cPlayerController.boostCooldown > 0f)
            {
                cPlayerController.boostCooldown = Mathf.Max(cPlayerController.boostCooldown - Time.fixedDeltaTime, 0f);
            }
            lastBoostInput = boostInput;
        }
    }
}
