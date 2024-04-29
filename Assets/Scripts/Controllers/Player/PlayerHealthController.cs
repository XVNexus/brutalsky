using Brutalsky;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
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

        // Local constants
        public const float DeathOffset = 1000f;

        // Local variables
        public float maxHealth;
        public float health = -1f;
        public bool alive = true;
        private float _lastSpeed;

        // Component references
        private Rigidbody2D _cRigidbody2D;

        // Linked modules
        [CanBeNull] private PlayerMovementController _mMovement;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnPlayerRespawn += OnPlayerRespawn;

            _cRigidbody2D = GetComponent<Rigidbody2D>();

            // Sync health with max health
            maxHealth = Master.Object.Health;
            health = maxHealth;
        }

        protected override void OnLink()
        {
            _mMovement = Master.GetSub<PlayerMovementController>("movement");
        }

        // Module functions
        public void Heal(float amount)
        {
            health = Mathf.Min(health + amount, maxHealth);
        }

        public void Hurt(float amount)
        {
            health = Mathf.Max(health - amount, 0f);
        }

        public void Revive()
        {
            if (alive) return;
            health = maxHealth;
            alive = true;
            if (_mMovement)
            {
                _mMovement.Unfreeze();
            }
        }

        public void Kill()
        {
            if (!alive) return;
            transform.position += new Vector3(DeathOffset, DeathOffset);
            health = 0f;
            alive = false;
            if (_mMovement)
            {
                _mMovement.Freeze();
            }
        }

        // Event functions
        private void OnPlayerRespawn(BsMap map, BsPlayer player)
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

            // Kill the player if out of map bounds
            if (MapSystem._.IsMapLoaded)
            {
                var position = transform.position;
                var mapSize = MapSystem._.ActiveMap.PlayArea;
                if (Mathf.Abs(position.x) > mapSize.x / 2f + MapSystem.BoundsMargin
                    || Mathf.Abs(position.y) > mapSize.y / 2f + MapSystem.BoundsMargin)
                {
                    Kill();
                }
            }

            // Kill the player if health reaches zero
            if (health == 0f)
            {
                Kill();
            }

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
