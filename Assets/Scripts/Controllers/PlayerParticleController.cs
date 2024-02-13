using System.Linq;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerParticleController : MonoBehaviour
    {
        // Variables
        private float lastSpeed;
        private float lastHealth;

        // References
        public ParticleSystem cBoostParticleSystem;
        public ParticleSystem cImpactParticleSystem;
        public ParticleSystem cHurtParticleSystem;
        public ParticleSystem cDeathParticleSystem;
        private Rigidbody2D cRigidbody2D;
        private OptionalComponent<PlayerHealthController> cPlayerHealthController;

        // Events
        private void Start()
        {
            cRigidbody2D = GetComponent<Rigidbody2D>();
            cPlayerHealthController =
                new OptionalComponent<PlayerHealthController>(GetComponent<PlayerHealthController>());
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            var impactForce = other.contacts.Sum(contact => contact.normalImpulse);
            var impactSpeed = lastSpeed;

            // Display impact particles
            var effectIntensity = Mathf.Min(MathfExt.TMP(impactForce, 25f, .5f, 1.5f) * .1f, 10f);
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
            if (!cPlayerHealthController.exists) return;
            var health = cPlayerHealthController.component.health;
            var deltaHealth = health - lastHealth;
            if (deltaHealth <= -1f)
            {
                var damage = (int)(-deltaHealth * 5f);
                var psEmission = cHurtParticleSystem.emission;
                var psBurst = psEmission.GetBurst(0);
                psBurst.count = Mathf.Min(damage, 100);
                psEmission.SetBurst(0, psBurst);
                cHurtParticleSystem.Play();
            }
            lastHealth = health;

            // Display death particles
            if (cPlayerHealthController.component.health != 0f || cDeathParticleSystem.isPlaying) return;
            cDeathParticleSystem.transform.SetParent(null, true);
            cDeathParticleSystem.Play();
            if (cPlayerHealthController.exists) lastHealth = cPlayerHealthController.component.health;
        }
    }
}
