using Controllers.Base;
using Controllers.Player;
using Data;
using Data.Object;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Controllers.Mount
{
    public class MountGrabController : SubControllerBase<BsMount>
    {
        // Controller metadata
        public override string Id => "grab";
        public override bool IsUnused => false;

        // Config options
        public float springForce;
        public float springDamping;
        public float cooldownTime;
        public float indicatorScale;
        public float animationTime;

        // Exposed properties
        public bool Active { get; private set; }
        public Vector2 Input { get; private set; }

        // Local variables
        private string _activePlayerId;
        private PlayerInputController _activePlayer;
        private Rigidbody2D _activeRigidbody;
        private bool _cooldown;
        private Vector2 _lastPosition;

        // External references
        public GameObject gIndicatorRing;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnPlayerDie += OnPlayerDie;
            EventSystem._.OnMapCleanup += OnMapCleanup;
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerDie -= OnPlayerDie;
            EventSystem._.OnMapCleanup -= OnMapCleanup;
        }

        // System functions
        public void GrabPlayer(PlayerInputController player)
        {
            // Set the tracker variables
            player.autoSendInput = false;
            Active = true;
            _activePlayerId = player.Master.Object.Id;
            _activePlayer = player;
            _activeRigidbody = player.gameObject.GetComponent<Rigidbody2D>();
            _lastPosition = transform.position;

            // Set the indicator ring to the player color and scale
            gIndicatorRing.LeanScale(Vector2.one * indicatorScale, animationTime)
                .setEaseOutCubic();
            gIndicatorRing.LeanColor(_activePlayer.Master.Object.Color.SetAlpha(.25f), animationTime)
                .setEaseOutCubic();
        }

        public void UngrabPlayer()
        {
            // Eject the player from the mount
            Input = Vector2.zero;
            _activeRigidbody.AddForce(MathfExt.ToVector(Master.Object.EjectionForce.x * Mathf.Deg2Rad,
                Master.Object.EjectionForce.y), ForceMode2D.Impulse);

            // Reset the tracker variables
            _activePlayer.autoSendInput = true;
            Active = false;
            _activePlayerId = "";
            _activePlayer = null;
            _activeRigidbody = null;

            // Reset the indicator ring
            gIndicatorRing.LeanScale(Vector2.one, animationTime)
                .setEaseOutCubic();
            gIndicatorRing.LeanColor(new Color(1f, 1f, 1f, .1f), animationTime)
                .setEaseOutCubic();

            // Temporarily lock the mount
            _cooldown = true;
            gameObject.LeanDelayedCall(cooldownTime, () =>
            {
                _cooldown = false;
            });
        }

        // Event functions
        private void OnPlayerDie(BsMap map, BsPlayer player)
        {
            // Ungrab the mounted player if they die
            if (player.Id != _activePlayerId) return;
            UngrabPlayer();
        }

        private void OnMapCleanup(BsMap map)
        {
            // Ungrab any mounted players before the map is unloaded
            if (Active)
            {
                UngrabPlayer();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Grab any players that come in contact
            if (Active || _cooldown || !other.CompareTag(Tags.PlayerTag)) return;
            _activePlayer = other.GetComponent<PlayerController>().GetSub<PlayerInputController>("input");
            if (_activePlayer == null) throw Errors.NoItemFound("player subcontroller", "input");
            GrabPlayer(_activePlayer);
        }

        private void FixedUpdate()
        {
            // Send player movement input to public variable for use in the logic system
            if (!Active) return;
            Input = _activePlayer.MovementInput;

            // Add force to the player to keep it within the mount
            var position = (Vector2)transform.position;
            _activeRigidbody.AddForce((transform.position - _activeRigidbody.transform.position)
                * springForce, ForceMode2D.Impulse);
            _activeRigidbody.AddForce(((position - _lastPosition) / Time.fixedDeltaTime - _activeRigidbody.velocity)
                * springDamping, ForceMode2D.Impulse);
            _lastPosition = position;

            // Ungrab the active player if the boost input is triggered
            if (!_activePlayer.BoostInput) return;
            UngrabPlayer();
        }
    }
}
