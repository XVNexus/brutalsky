using Brutalsky;
using JetBrains.Annotations;
using UnityEngine;
using Utils;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerHealthController : SubControllerBase<BsPlayer>
    {
        public override string Id => "health";
        public override bool IsUnused => false;

        public const float DeathOffset = 1000f;

        public float maxHealth;
        public float health = -1f;
        public bool alive = true;
        private float _lastSpeed;

        private Rigidbody2D _cRigidbody2D;

        [CanBeNull] private PlayerMovementController _mMovement;

        protected override void OnInit()
        {
            _cRigidbody2D = GetComponent<Rigidbody2D>();

            // Sync health with max health
            health = maxHealth;
        }

        protected override void OnLink()
        {
            _mMovement = Master.GetSub<PlayerMovementController>("movement");
        }

        private void OnCollisionEnter2D(Collision2D other) => OnCollision(other);

        private void OnCollisionStay2D(Collision2D other) => OnCollision(other);

        private void OnCollision(Collision2D other)
        {
            if (!alive) return;

            // Get collision info
            var impactForce = other.TotalNormalImpulse() * (other.gameObject.CompareTag(Tags.Player) ? 2f : 1f);
            if (impactForce < 25f) return;
            var impactSpeed = _lastSpeed;

            // Apply damage to player
            var damageApplied = BsPlayer.CalculateDamage(impactForce);
            var damageMultiplier = Mathf.Min(1f / (impactSpeed * .2f), 1f);
            Hurt(damageApplied * damageMultiplier);
        }

        public void Heal(float amount)
        {
            health = Mathf.Min(health + amount, maxHealth);
        }

        public void Hurt(float amount)
        {
            health = Mathf.Max(health - amount, 0f);
        }

        public void Refresh()
        {
            if (alive)
            {
                health = maxHealth;
            }
            else
            {
                Revive();
            }
            _cRigidbody2D.velocity = Vector2.zero;
            if (_mMovement)
            {
                _mMovement.boostCharge = 0f;
                _mMovement.boostCooldown = 0f;
            }
        }

        public bool Revive()
        {
            if (alive) return false;
            transform.position -= new Vector3(DeathOffset, DeathOffset);
            if (_mMovement) _mMovement.Unfreeze();
            health = maxHealth;
            alive = true;
            return true;
        }

        public bool Kill()
        {
            if (!alive) return false;
            transform.position += new Vector3(DeathOffset, DeathOffset);
            if (_mMovement) _mMovement.Freeze();
            health = 0f;
            alive = false;
            return true;
        }

        private void FixedUpdate()
        {
            // Kill the player if health reaches zero
            if (alive && health == 0)
            {
                Debug.Log("owie 1");
                Kill();
            }

            // Save the current speed for future reference
            _lastSpeed = _cRigidbody2D.velocity.magnitude;
        }
    }
}
