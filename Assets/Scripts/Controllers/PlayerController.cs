using Brutalsky;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        // Constants
        public const string Tag = "Player";
        public const float DeathOffset = 1000f;
        public const int MaxOnGroundFrames = 5;

        // Source
        public BsPlayer bsObject;

        // Variables
        public float maxHealth;
        public float health = -1f;
        public bool alive = true;
        public Color color;
        public float boostCharge;
        public float boostCooldown;
        public bool onGround;
        private int onGroundFrames;
        private float lastPositionY;

        // References
        public Light2D cLight2D;
        public SpriteRenderer cRingSpriteRenderer;
        public ParticleSystem[] cParticleSystems;
        private SpriteRenderer cPlayerSpriteRenderer;
        private Rigidbody2D cRigidbody2D;
        private CircleCollider2D cCircleCollider2D;

        // Controls
        public InputAction iMovement;
        public InputAction iBoost;

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
            cRigidbody2D.velocity = Vector2.zero;
            boostCharge = 0f;
            boostCooldown = 0f;
        }

        public bool Revive()
        {
            if (alive) return false;
            transform.position -= new Vector3(DeathOffset, DeathOffset);
            Unfreeze();
            health = maxHealth;
            alive = true;
            return true;
        }

        public bool Kill()
        {
            if (!alive) return false;
            transform.position += new Vector3(DeathOffset, DeathOffset);
            Freeze();
            health = 0f;
            alive = false;
            return true;
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

            iMovement = EventSystem.current.inputActionAsset.FindAction("Movement");
            iMovement.Enable();
            iBoost = EventSystem.current.inputActionAsset.FindAction("Boost");
            iBoost.Enable();
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollision(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            OnCollision(other);
        }

        private void OnCollision(Collision2D other)
        {
            // Update ground status
            if (!other.gameObject.CompareTag(ShapeController.Tag) && !other.gameObject.CompareTag(Tag)) return;
            if (other.GetContact(0).point.y > lastPositionY - .25f) return;
            onGroundFrames = MaxOnGroundFrames;
            onGround = true;
        }

        // Updates
        private void FixedUpdate()
        {
            // Update ground status
            onGround = onGroundFrames > 0;
            onGroundFrames = Mathf.Max(onGroundFrames - 1, 0);
            lastPositionY = transform.position.y;

            // Kill the player if health reaches zero
            if (alive && health == 0)
            {
                Kill();
            }
        }
    }
}
