using Brutalsky;
using Brutalsky.Object;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerParticleController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "particle";
        public override bool IsUnused => false;

        // Local constants
        public const float BoostThreshold = 30f;
        public const float BoostCap = 1000f;
        public const float ParticleMultiplier = 3f;

        // Local variables
        private float _lastSpeed;
        private int _lastHealth = -1;

        // Component references
        public ParticleSystem cTouchParticleSystem;
        public ParticleSystem cSlideParticleSystem;
        public ParticleSystem cBoostParticleSystem;
        public ParticleSystem cImpactParticleSystem;
        public ParticleSystem cHealParticleSystem;
        public ParticleSystem cHurtParticleSystem;
        public ParticleSystem cDeathParticleSystem;
        private ParticleSystem[] _cAllParticleSystems;
        private Rigidbody2D _cRigidbody2D;
        private SpriteRenderer _cSpriteRenderer;

        // Linked modules
        [CanBeNull] private PlayerHealthController _mHealth;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnPlayerSpawn += OnPlayerSpawn;

            _cRigidbody2D = GetComponent<Rigidbody2D>();
            _cSpriteRenderer = GetComponent<SpriteRenderer>();

            // Add all particle systems to a group for convenience
            _cAllParticleSystems = new[]
            {
                cTouchParticleSystem, cSlideParticleSystem, cBoostParticleSystem, cImpactParticleSystem,
                cHealParticleSystem, cHurtParticleSystem, cDeathParticleSystem
            };

            // Sync particle system colors with player color
            foreach (var cParticleSystem in _cAllParticleSystems)
            {
                cParticleSystem.GetComponent<Renderer>().material.color = _cSpriteRenderer.color;
            }
        }

        protected override void OnLink()
        {
            _mHealth = Master.GetSub<PlayerHealthController>("health");
        }

        // Module functions
        public void DisplayTouchParticles(float contactAngle)
        {
            cTouchParticleSystem.transform.localRotation = Quaternion.Euler(0f, 0f, contactAngle);
            cTouchParticleSystem.Play();
        }

        public void DisplaySlideParticles(float contactAngle)
        {
            cSlideParticleSystem.transform.localRotation = Quaternion.Euler(0f, 0f, contactAngle);
            cSlideParticleSystem.Play();
        }

        public void DisplayBoostParticles(float speed)
        {
            if (speed > BoostCap) return;
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
            var effectIntensity = Mathf.Min(PlayerHealthController.CalculateDamage(impactForce) * .2f, 10f);
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
            if (!MapSystem._.MapLoaded) return;
            cDeathParticleSystem.gameObject.SetActive(true);
            var mapBounds = MapSystem._.ActiveMap.PlayArea * .5f + Vector2.one;
            cDeathParticleSystem.transform.position = new Vector2
            (
                Mathf.Clamp(transform.position.x, -mapBounds.x, mapBounds.x),
                Mathf.Clamp(transform.position.y, -mapBounds.y, mapBounds.y)
            );
            cDeathParticleSystem.Play();
        }

        // Event functions
        private void OnPlayerSpawn(BsMap map, BsPlayer player)
        {
            if (player.Id != Master.Object.Id) return;
            foreach (var cParticleSystem in _cAllParticleSystems)
            {
                cParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            _lastSpeed = 0f;
            _lastHealth = -1;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            DisplayImpactParticles(other.TotalNormalImpulse());
            if (!other.gameObject.CompareTag(Tags.Shape) || other.DirectnessFactor() < .5f) return;
            DisplayTouchParticles(MathfExt.Atan2(
                other.GetContact(0).point - (Vector2)transform.position) * Mathf.Rad2Deg);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag(Tags.Shape) || other.relativeVelocity.magnitude < 3f) return;
            DisplaySlideParticles(MathfExt.Atan2(
                other.GetContact(0).point - (Vector2)transform.position) * Mathf.Rad2Deg);
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
            if (_lastHealth < 0)
            {
                _lastHealth = health;
                return;
            }
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
