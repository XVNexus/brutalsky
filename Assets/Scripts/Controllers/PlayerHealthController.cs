using System.Linq;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerHealthController : MonoBehaviour
    {
        // Settings
        public float maxHealth = 100f;
        public float health = 0f;

        // Variables
        private float lastSpeed;
        private bool isDead;

        // References
        private Rigidbody2D cRigidbody2D;
    
        // Events
        private void Start()
        {
            cRigidbody2D = GetComponent<Rigidbody2D>();

            // Sync health with max health
            health = maxHealth;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            var impactForce = other.contacts.Sum(contact => contact.normalImpulse);
            var impactSpeed = lastSpeed;

            // Apply damage to player
            var damageApplied = MathfExt.TMP(impactForce, 25f, .5f, 1.5f);
            var damageMultiplier = Mathf.Min(1f / (impactSpeed * .2f), 1f);
            var damage = damageApplied * damageMultiplier;
            health = Mathf.Max(health - damage, 0f);
            isDead = health == 0f;
        }

        // Updates
        private void FixedUpdate()
        {
            // Save the current speed for future reference
            lastSpeed = cRigidbody2D.velocity.magnitude;

            // Delete the player if dead for more than a frame
            if (isDead)
            {
                Destroy(gameObject);
            }
        }
    }
}
