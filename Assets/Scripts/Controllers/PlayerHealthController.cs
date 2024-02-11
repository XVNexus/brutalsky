using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerHealthController : MonoBehaviour
    {
        /// <summary>
        /// The maximum possible player health
        /// </summary>
        public float maxHealth = 100f;
        /// <summary>
        /// The starting player health
        /// </summary>
        public float health = 100f;

        private Rigidbody2D cRigidbody2D;
        private float speedLastFrame = 0f;
    
        private void Start()
        {
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            speedLastFrame = cRigidbody2D.velocity.magnitude;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var impactForce = other.relativeVelocity.magnitude;
            var impactSpeed = speedLastFrame;

            var damageApplied = MathfExt.TMP(impactForce, 25f, .5f, 1.5f);
            var damageMultiplier = Mathf.Min(1f / (impactSpeed * .2f), 1f);

            var damage = damageApplied * damageMultiplier;

            health = Mathf.Max(health - damage, 0f);
        }
    }
}
