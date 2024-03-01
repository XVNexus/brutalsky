using System.Linq;
using Brutalsky;
using UnityEngine;

namespace Controllers.Player
{
    public class PlayerParticleController : MonoBehaviour
    {
        // Constants
        public const float BoostThreshold = 40f;
        public const float ParticleMultiplier = 5f;

        // Variables
        private float lastSpeed;
        private float lastHealth = -1f;

        // References
        public ParticleSystem cBoostParticleSystem;
        public ParticleSystem cImpactParticleSystem;
        public ParticleSystem cHurtParticleSystem;
        public ParticleSystem cDeathParticleSystem;
        private PlayerController cPlayerController;
        private Rigidbody2D cRigidbody2D;

        // Functions
        public void DisplayBoostParticles(float speed)
        {
            switch (speed)
            {
                case >= BoostThreshold when lastSpeed < BoostThreshold:
                    cBoostParticleSystem.Play();
                    break;
                case < BoostThreshold when lastSpeed >= BoostThreshold:
                    cBoostParticleSystem.Stop();
                    break;
            }
        }

        public void DisplayImpactParticles(float impactForce)
        {
            var effectIntensity = Mathf.Min(BsPlayer.CalculateDamage(impactForce) * .1f, 10f);
            if (effectIntensity < 3f) return;
            var psMain = cImpactParticleSystem.main;
            psMain.startSize = effectIntensity;
            psMain.startLifetime = effectIntensity * .1f;
            cImpactParticleSystem.Play();
        }

        public void DisplayHurtParticles(float deltaHealth)
        {
            if (deltaHealth >= 0f) return;
            var particleCount = (int)(-deltaHealth * ParticleMultiplier);
            var psEmission = cHurtParticleSystem.emission;
            var psBurst = psEmission.GetBurst(0);
            psBurst.count = Mathf.Min(particleCount, (int)(1000 * Time.fixedDeltaTime));
            psEmission.SetBurst(0, psBurst);
            cHurtParticleSystem.Play();
        }

        public void DisplayDeathParticles()
        {
            cDeathParticleSystem.Play();
        }

        // Events
        private void OnEnable()
        {
            cPlayerController = GetComponent<PlayerController>();
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Offset death particle system by the opposite amount the player is moved on death
            // This ensures that death particles will play where the player was before dying
            cDeathParticleSystem.transform.localPosition =
                new Vector3(-PlayerController.DeathOffset, -PlayerController.DeathOffset);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            DisplayImpactParticles(other.contacts.Sum(contact => contact.normalImpulse));
        }

        // Updates
        private void FixedUpdate()
        {
            var speed = cRigidbody2D.velocity.magnitude;
            DisplayBoostParticles(speed);
            lastSpeed = speed;

            var health = Mathf.Floor(cPlayerController.health * ParticleMultiplier) / ParticleMultiplier;
            DisplayHurtParticles(health - lastHealth);
            if (health == 0f && lastHealth > 0f)
            {
                DisplayDeathParticles();
            }
            lastHealth = health;
        }
    }
}
