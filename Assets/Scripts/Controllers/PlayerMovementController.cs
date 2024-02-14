using System;
using System.Linq;
using UnityEngine;

namespace Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        // Settings
        public float moveForce = 25f;

        // Variables
        public float boostCharge;
        public float boostCooldown;
        private bool lastBoostInput;

        // References
        private Rigidbody2D cRigidbody2D;
    
        // Events
        private void Start()
        {
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
            var boostInput = Input.GetButton("Jump") && boostCooldown == 0f;
            if (boostInput)
            {
                boostCharge = Mathf.Min(boostCharge + Time.fixedDeltaTime, 3f);
            }
            else if (lastBoostInput)
            {
                var boost = Mathf.Pow(boostCharge, 2f) + 2f;
                boostCooldown = boost;
                boostCharge = 0f;
                cRigidbody2D.velocity *= boost;
            }
            if (boostCooldown > 0f)
            {
                boostCooldown = Mathf.Max(boostCooldown - Time.fixedDeltaTime, 0f);
            }
            lastBoostInput = boostInput;
        }
    }
}
