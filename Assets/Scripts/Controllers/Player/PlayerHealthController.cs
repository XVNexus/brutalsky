using Brutalsky;
using Brutalsky.Object;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerHealthController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "health";
        public override bool IsUnused => Mathf.Approximately(Master.Object.Health, -1f);

        // Local variables
        public float maxHealth;
        public float health = -1f;
        public bool alive = true;
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
            maxHealth = Master.Object.Health;
            health = maxHealth;
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
        }

        // Module functions
        public void Heal(float amount)
        {
            health = Mathf.Min(health + amount, maxHealth);
        }

        public void Hurt(float amount)
        {
            if (amount >= health)
            {
                Kill();
                return;
            }
            health = Mathf.Max(health - amount, 0f);
        }

        public void Revive()
        {
            if (alive) return;
            health = maxHealth;
            alive = true;
            _cRigidbody2D.simulated = true;
            _cSpriteRenderer.enabled = true;
            _cCircleCollider2D.enabled = true;
            gameObject.SetChildrenActive(true);
        }

        public void Kill()
        {
            if (!alive) return;
            health = 0f;
            alive = false;
            _cRigidbody2D.simulated = false;
            _cSpriteRenderer.enabled = false;
            _cCircleCollider2D.enabled = false;
            gameObject.SetChildrenActive(false);
            EventSystem._.EmitPlayerDie(MapSystem._.ActiveMap, Master.Object);
        }

        // Event functions
        private void OnPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visible)
        {
            if (player.Id != Master.Object.Id) return;
            maxHealth = player.Health;
            health = maxHealth;
            Revive();
        }

        private void OnCollisionEnter2D(Collision2D other) => OnCollision(other);

        private void OnCollisionStay2D(Collision2D other) => OnCollision(other);

        private void OnCollision(Collision2D other)
        {
            if (!alive) return;

            // Get collision info
            var impactForce = other.TotalNormalImpulse() * (other.gameObject.CompareTag(Tags.PlayerTag) ? 2f : 1f);
            if (impactForce < 25f) return;
            var impactSpeed = _lastSpeed;

            // Apply damage to player
            var damageApplied = CalculateDamage(impactForce);
            var damageMultiplier = Mathf.Min(1f / (impactSpeed * .2f), 1f);
            Hurt(damageApplied * damageMultiplier);
        }

        private void FixedUpdate()
        {
            if (!alive) return;

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
