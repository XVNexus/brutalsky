using Brutalsky;
using UnityEngine;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerParticleController : MonoBehaviour
    {
        // Constants
        public const float BoostThreshold = 30f;
        public const float ParticleMultiplier = 3f;

        // Variables
        private float lastSpeed;
        private int lastHealth = -1;

        // References
        public ParticleSystem cTouchParticleSystem;
        public ParticleSystem cSlideParticleSystem;
        public ParticleSystem cBoostParticleSystem;
        public ParticleSystem cImpactParticleSystem;
        public ParticleSystem cHealParticleSystem;
        public ParticleSystem cHurtParticleSystem;
        public ParticleSystem cDeathParticleSystem;
        private PlayerController cPlayerController;
        private Rigidbody2D cRigidbody2D;

        // Functions
        public void DisplayTouchParticles(float contactAngle, Material shapeMaterial)
        {
            cTouchParticleSystem.GetComponent<Renderer>().material = shapeMaterial;
            cTouchParticleSystem.transform.localRotation = Quaternion.Euler(0f, 0f, contactAngle);
            cTouchParticleSystem.Play();
        }

        public void DisplaySlideParticles(float contactAngle, Material shapeMaterial)
        {
            cSlideParticleSystem.GetComponent<Renderer>().material = shapeMaterial;
            cSlideParticleSystem.transform.localRotation = Quaternion.Euler(0f, 0f, contactAngle);
            cSlideParticleSystem.Play();
        }

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
            var effectIntensity = Mathf.Min(BsPlayer.CalculateDamage(impactForce) * .2f, 10f);
            if (effectIntensity < 3f) return;
            var psMain = cImpactParticleSystem.main;
            psMain.startSize = effectIntensity;
            psMain.startLifetime = effectIntensity * .1f;
            cImpactParticleSystem.Play();
        }

        public void DisplayHealHurtParticles(int deltaHealth)
        {
            var healHurtParticleSystem = deltaHealth > 0f ? cHealParticleSystem : cHurtParticleSystem;
            var psEmission = healHurtParticleSystem.emission;
            var psBurst = psEmission.GetBurst(0);
            psBurst.count = Mathf.Min(Mathf.Abs(deltaHealth), (int)(1000 * Time.fixedDeltaTime));
            psEmission.SetBurst(0, psBurst);
            healHurtParticleSystem.Play();
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
            DisplayImpactParticles(other.TotalNormalImpulse());
            if (!other.gameObject.CompareTag(ShapeController.Tag) || other.DirectnessFactor() < .5f) return;
            DisplayTouchParticles(MathfExt.Atan2(
                other.GetContact(0).point - (Vector2)transform.position) * Mathf.Rad2Deg,
                other.gameObject.GetComponent<MeshRenderer>().material);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag(ShapeController.Tag) || other.relativeVelocity.magnitude < 3f) return;
            DisplaySlideParticles(MathfExt.Atan2(
                other.GetContact(0).point - (Vector2)transform.position) * Mathf.Rad2Deg,
                other.gameObject.GetComponent<MeshRenderer>().material);
        }

        // Updates
        private void FixedUpdate()
        {
            var speed = cRigidbody2D.velocity.magnitude;
            DisplayBoostParticles(speed);
            lastSpeed = speed;

            var health = Mathf.FloorToInt(cPlayerController.health * ParticleMultiplier);
            var deltaHealth = health - lastHealth;
            if (deltaHealth == 0) return;
            DisplayHealHurtParticles(deltaHealth);
            if (health == 0 && lastHealth > 0)
            {
                DisplayDeathParticles();
            }
            lastHealth = health;
        }
    }
}
