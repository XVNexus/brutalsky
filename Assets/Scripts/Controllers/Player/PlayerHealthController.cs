using Controllers.Base;
using Data;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Controllers.Player
{
    public class PlayerHealthController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "health";
        public override bool IsUnused => Mathf.Approximately(Master.Object.Health, -1f);

        // Exposed properties
        public float MaxHealth { get; private set; }
        public float Health { get; private set; } = -1f;
        public bool Alive { get; private set; } = true;

        // Local variables
        private float _lastSpeed;

        // External references
        private Rigidbody2D _cRigidbody2D;
        private SpriteRenderer _cSpriteRenderer;
        private CircleCollider2D _cCircleCollider2D;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnPlayerSpawn += OnPlayerSpawn;

            _cRigidbody2D = GetComponent<Rigidbody2D>();
            _cSpriteRenderer = GetComponent<SpriteRenderer>();
            _cCircleCollider2D = GetComponent<CircleCollider2D>();

            // Sync health with max health
            MaxHealth = Master.Object.Health;
            Health = MaxHealth;
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
        }

        // Module functions
        public void Heal(float amount)
        {
            Health = Mathf.Min(Health + amount, MaxHealth);
        }

        public void Hurt(float amount)
        {
            if (amount >= Health)
            {
                Kill();
                return;
            }
            Health = Mathf.Max(Health - amount, 0f);
        }

        public void Revive()
        {
            if (Alive) return;
            Health = MaxHealth;
            Alive = true;
            _cRigidbody2D.simulated = true;
            _cCircleCollider2D.enabled = true;
            _cSpriteRenderer.enabled = true;
            gameObject.SetChildrenActive(true);
        }

        public void Kill()
        {
            if (!Alive) return;
            Health = 0f;
            Alive = false;
            _cRigidbody2D.simulated = false;
            _cCircleCollider2D.enabled = false;
            _cSpriteRenderer.enabled = false;
            gameObject.SetChildrenActive(false);
            EventSystem._.EmitPlayerDie(MapSystem._.ActiveMap, Master.Object);
        }

        // Event functions
        private void OnPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visible)
        {
            if (player.Id != Master.Object.Id) return;
            MaxHealth = player.Health;
            Health = MaxHealth;
            Revive();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!Alive) return;

            OnCollision(other.gameObject.CompareTag(Tags.PlayerTag)
                ? _lastSpeed + other.gameObject.GetComponent<PlayerHealthController>()._lastSpeed
                : other.TotalNormalImpulse());
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!Alive) return;

            // Get collision info
            OnCollision(other.TotalNormalImpulse());
        }

        private void OnCollision(float impactForce)
        {
            // Apply damage to player
            if (impactForce < 25f) return;
            var impactSpeed = _lastSpeed;
            var damageApplied = CalculateDamage(impactForce);
            var damageMultiplier = Mathf.Min(1f / (impactSpeed * .2f), 1f);
            Hurt(damageApplied * damageMultiplier);
        }

        private void FixedUpdate()
        {
            if (!Alive) return;

            // Save the current speed for future reference
            _lastSpeed = _cRigidbody2D.velocity.magnitude;
        }

        // Utility functions
        public static float CalculateDamage(float impactForce)
        {
            return Mathf.Max(impactForce - 20f, 0) * .5f;
        }
    }
}
