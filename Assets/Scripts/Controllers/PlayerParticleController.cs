using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerParticleController : MonoBehaviour
    {
        public float lastSpeed;
        public float lastHealth;

        public ParticleSystem cBoostParticleSystem;
        public ParticleSystem cImpactParticleSystem;
        public ParticleSystem cHurtParticleSystem;
        public ParticleSystem cDeathParticleSystem;

        private Rigidbody2D cRigidbody2D;
        private OptionalComponent<PlayerHealthController> cPlayerHealthController;

        private void Start()
        {
            cRigidbody2D = GetComponent<Rigidbody2D>();
            cPlayerHealthController =
                new OptionalComponent<PlayerHealthController>(GetComponent<PlayerHealthController>());
        }

        private void FixedUpdate()
        {
            // Display boost particles
            var speed = cRigidbody2D.velocity.magnitude;
            const float threshold = 40f;
            if (speed >= threshold && lastSpeed < threshold)
            {
                cBoostParticleSystem.Play();
            }
            else if (speed < threshold && lastSpeed >= threshold)
            {
                cBoostParticleSystem.Stop();
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
                psBurst.count = damage;
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            var impactSpeed = lastSpeed;

            // Display impact particles
            var effectIntensity = Mathf.Min(MathfExt.TMP(impactSpeed, 25f, .25f, 1.5f) * .25f, 10f);
            if (effectIntensity < 3f) return;
            var psMain = cImpactParticleSystem.main;
            psMain.startSize = effectIntensity;
            psMain.startLifetime = effectIntensity * .1f;
            cImpactParticleSystem.Play();
        }
    }
}
