using Brutalsky;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        // Source
        public BsPlayer bsObject;

        // Variables
        public float maxHealth = 100f;
        public float health = -1f;
        public Color color;
        public float boostCharge;
        public float boostCooldown;
        public bool alive = true;

        // References
        public Light2D cLight2D;
        public SpriteRenderer cRingSpriteRenderer;
        public ParticleSystem[] cParticleSystems;
        private SpriteRenderer cPlayerSpriteRenderer;

        // Functions
        public void Heal(float amount)
        {
            health = Mathf.Min(health + amount, maxHealth);
        }

        public void Hurt(float amount)
        {
            health = Mathf.Max(health - amount, 0f);
        }

        // Events
        private void Start()
        {
            cPlayerSpriteRenderer = GetComponent<SpriteRenderer>();
            
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
            if (health == 0f && alive)
            {
                alive = false;
            }
            else if (!alive)
            {
                Destroy(bsObject.instanceObject);
            }
        }
    }
}
