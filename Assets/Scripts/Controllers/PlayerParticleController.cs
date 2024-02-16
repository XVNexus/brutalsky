using System.Linq;
using Brutalsky;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerParticleController : MonoBehaviour
    {
        // Variables
        public float particleMultiplier = 5f;
        private float lastSpeed;
        private float lastHealth = -1f;

        // References
        public PlayerController cPlayerController;
        public ParticleSystem cBoostParticleSystem;
        public ParticleSystem cImpactParticleSystem;
        public ParticleSystem cHurtParticleSystem;
        public ParticleSystem cDeathParticleSystem;
        private Rigidbody2D cRigidbody2D;

        // Events
        private void Start()
        {
            cPlayerController = GetComponent<PlayerController>();
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            var impactForce = other.contacts.Sum(contact => contact.normalImpulse);

            // Display impact particles
            var effectIntensity = Mathf.Min(BsPlayer.CalculateDamage(impactForce) * .1f, 10f);
            if (effectIntensity < 3f) return;
            var psMain = cImpactParticleSystem.main;
            psMain.startSize = effectIntensity;
            psMain.startLifetime = effectIntensity * .1f;
            cImpactParticleSystem.Play();
        }

        // Updates
        private void FixedUpdate()
        {
            // Display boost particles
            var speed = cRigidbody2D.velocity.magnitude;
            const float threshold = 40f;
            switch (speed)
            {
                case >= threshold when lastSpeed < threshold:
                    cBoostParticleSystem.Play();
                    break;
                case < threshold when lastSpeed >= threshold:
                    cBoostParticleSystem.Stop();
                    break;
            }
            lastSpeed = speed;

            // Display hurt particles
            var health = Mathf.Ceil(cPlayerController.health * particleMultiplier) / particleMultiplier;
            var deltaHealth = health - lastHealth;
            if (deltaHealth < 0f)
            {
                var particleCount = (int)(-deltaHealth * particleMultiplier);
                var psEmission = cHurtParticleSystem.emission;
                var psBurst = psEmission.GetBurst(0);
                psBurst.count = Mathf.Min(particleCount, (int)(100 * particleMultiplier * Time.fixedDeltaTime));
                psEmission.SetBurst(0, psBurst);
                cHurtParticleSystem.Play();
            }
            lastHealth = health;

            // Display death particles
            if (health == 0f && cDeathParticleSystem.isStopped)
            {
                cDeathParticleSystem.transform.SetParent(null, true);
                cDeathParticleSystem.Play();
            }
        }
    }
}
