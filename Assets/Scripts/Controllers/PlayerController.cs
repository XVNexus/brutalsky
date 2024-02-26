using System;
using Brutalsky;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        // Constants
        public const float DeathOffset = 1000f;

        // Source
        public BsPlayer bsObject;

        // Variables
        public float maxHealth = 100f;
        public float health = -1f;
        public bool alive = true;
        public Color color;
        public float boostCharge;
        public float boostCooldown;

        // References
        public Light2D cLight2D;
        public SpriteRenderer cRingSpriteRenderer;
        public ParticleSystem[] cParticleSystems;
        private SpriteRenderer cPlayerSpriteRenderer;
        private Rigidbody2D cRigidbody2D;
        private CircleCollider2D cCircleCollider2D;

        // Functions
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
        }

        public void Revive()
        {
            transform.position -= new Vector3(DeathOffset, DeathOffset);
            Unfreeze();
            health = maxHealth;
            alive = true;
        }

        public void Kill()
        {
            transform.position += new Vector3(DeathOffset, DeathOffset);
            Freeze();
            alive = false;
        }

        public void Freeze()
        {
            cRigidbody2D.simulated = false;
            cCircleCollider2D.enabled = false;
        }

        public void Unfreeze()
        {
            cRigidbody2D.simulated = true;
            cCircleCollider2D.enabled = true;
        }

        // Events
        private void OnEnable()
        {
            cPlayerSpriteRenderer = GetComponent<SpriteRenderer>();
            cRigidbody2D = GetComponent<Rigidbody2D>();
            cCircleCollider2D = GetComponent<CircleCollider2D>();
        }

        private void Start()
        {
            // Sync health with max health
            health = maxHealth;

            // Set all colored items to match the player color
            cPlayerSpriteRenderer.color = color;
            cLight2D.color = color;
            cRingSpriteRenderer.color = color;
            foreach (var cParticleSystem in cParticleSystems)
            {
                var psMain = cParticleSystem.main;
                psMain.startColor = color;
            }
        }

        // Updates
        private void FixedUpdate()
        {
            // Kill the player if health reaches zero
            if (alive && health == 0)
            {
                Kill();
            }
        }
    }
}
