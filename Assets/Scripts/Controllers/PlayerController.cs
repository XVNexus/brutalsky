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
        public BsPlayer BsObject;

        // Variables
        public float maxHealth;
        public float health = -1f;
        public bool alive = true;
        public Color color;
        public float boostCharge;
        public float boostCooldown;
        public bool onGround;
        private int _onGroundFrames;
        private float _lastPositionY;

        // References
        public Light2D cLight2D;
        public SpriteRenderer cRingSpriteRenderer;
        public ParticleSystem[] cParticleSystems;
        private SpriteRenderer _cPlayerSpriteRenderer;
        private Rigidbody2D _cRigidbody2D;
        private CircleCollider2D _cCircleCollider2D;

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
            _cRigidbody2D.velocity = Vector2.zero;
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
            _cRigidbody2D.simulated = false;
            _cCircleCollider2D.enabled = false;
        }

        public void Unfreeze()
        {
            _cRigidbody2D.simulated = true;
            _cCircleCollider2D.enabled = true;
        }

        // Events
        private void Start()
        {
            _cPlayerSpriteRenderer = GetComponent<SpriteRenderer>();
            _cRigidbody2D = GetComponent<Rigidbody2D>();
            _cCircleCollider2D = GetComponent<CircleCollider2D>();

            iMovement = EventSystem._.inputActionAsset.FindAction("Movement");
            iMovement.Enable();
            iBoost = EventSystem._.inputActionAsset.FindAction("Boost");
            iBoost.Enable();

            // Sync health with max health
            health = maxHealth;

            // Set all colored items to match the player color
            _cPlayerSpriteRenderer.color = color;
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
            if (other.GetContact(0).point.y > _lastPositionY - .25f) return;
            _onGroundFrames = MaxOnGroundFrames;
            onGround = true;
        }

        // Updates
        private void FixedUpdate()
        {
            // Update ground status
            onGround = _onGroundFrames > 0;
            _onGroundFrames = Mathf.Max(_onGroundFrames - 1, 0);
            _lastPositionY = transform.position.y;

            // Kill the player if health reaches zero
            if (alive && health == 0)
            {
                Kill();
            }
        }
    }
}
