using Brutalsky;
using JetBrains.Annotations;
using UnityEngine;
using Utils;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerParticleController : SubControllerBase<BsPlayer>
    {
        public override string Id => "particle";
        public override bool IsUnused => false;

        public const float BoostThreshold = 30f;
        public const float ParticleMultiplier = 3f;

        private float _lastSpeed;
        private int _lastHealth = -1;

        public ParticleSystem cTouchParticleSystem;
        public ParticleSystem cSlideParticleSystem;
        public ParticleSystem cBoostParticleSystem;
        public ParticleSystem cImpactParticleSystem;
        public ParticleSystem cHealParticleSystem;
        public ParticleSystem cHurtParticleSystem;
        public ParticleSystem cDeathParticleSystem;
        private Rigidbody2D _cRigidbody2D;
        private SpriteRenderer _cSpriteRenderer;

        [CanBeNull] private PlayerHealthController _mHealth;

        protected override void OnInit()
        {
            _cRigidbody2D = GetComponent<Rigidbody2D>();
            _cSpriteRenderer = GetComponent<SpriteRenderer>();

            // Sync particle system colors with player color
            foreach (var coloredParticleSystem in new[] { cBoostParticleSystem, cImpactParticleSystem,
                cHealParticleSystem, cHurtParticleSystem, cDeathParticleSystem })
            {
                coloredParticleSystem.GetComponent<Renderer>().material.color = _cSpriteRenderer.color;
            }

            // Shift death particle system to ensure death particles play where the player originally was
            cDeathParticleSystem.transform.localPosition =
                new Vector3(-PlayerHealthController.DeathOffset, -PlayerHealthController.DeathOffset);
        }

        protected override void OnLink()
        {
            _mHealth = Master.GetSub<PlayerHealthController>("health");
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            DisplayImpactParticles(other.TotalNormalImpulse());
            if (!other.gameObject.CompareTag(Tags.Shape) || other.DirectnessFactor() < .5f) return;
            DisplayTouchParticles(MathfExt.Atan2(
                other.GetContact(0).point - (Vector2)transform.position) * Mathf.Rad2Deg,
                other.gameObject.GetComponent<MeshRenderer>().material);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag(Tags.Shape) || other.relativeVelocity.magnitude < 3f) return;
            DisplaySlideParticles(MathfExt.Atan2(
                other.GetContact(0).point - (Vector2)transform.position) * Mathf.Rad2Deg,
                other.gameObject.GetComponent<MeshRenderer>().material);
        }

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
                case >= BoostThreshold when _lastSpeed < BoostThreshold:
                    cBoostParticleSystem.Play();
                    break;
                case < BoostThreshold when _lastSpeed >= BoostThreshold:
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

        private void FixedUpdate()
        {
            // Display boost particles
            var speed = _cRigidbody2D.velocity.magnitude;
            DisplayBoostParticles(speed);
            _lastSpeed = speed;

            // Display heal/hurt particles
            if (!_mHealth) return;
            var health = Mathf.CeilToInt(_mHealth.health * ParticleMultiplier);
            var deltaHealth = health - _lastHealth;
            if (deltaHealth == 0) return;
            DisplayHealHurtParticles(deltaHealth);
            if (health == 0 && _lastHealth > 0)
            {
                DisplayDeathParticles();
            }
            _lastHealth = health;
        }
    }
}
