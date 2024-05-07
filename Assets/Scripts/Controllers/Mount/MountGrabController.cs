using Brutalsky;
using Brutalsky.Object;
using Controllers.Base;
using Controllers.Player;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;

namespace Controllers.Mount
{
    public class MountGrabController : SubControllerBase<BsMount>
    {
        // Controller metadata
        public override string Id => "grab";
        public override bool IsUnused => !Master.Object.Simulated;

        // Local constants
        public const float CooldownTime = 1f;
        public const float IndicatorScale = 1.6f / .9f;
        public const float AnimTime = .25f;

        // Local variables
        public bool active;
        public Vector2 input;
        private string _activePlayerId;
        private PlayerInputController _activePlayer;
        private Rigidbody2D _activeRigidbody;
        private RelativeJoint2D _activeClamp;
        private bool _cooldown;

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
            active = true;
            _activePlayerId = player.Master.Object.Id;
            _activePlayer = player;
            _activeRigidbody = player.gameObject.GetComponent<Rigidbody2D>();

            // Lock the player to the mount
            player.autoSendInput = false;
            _activeClamp = gameObject.AddComponent<RelativeJoint2D>();
            _activeClamp.connectedBody = _activeRigidbody;
            _activeClamp.autoConfigureOffset = false;
            _activeClamp.linearOffset = Vector2.zero;
            _activeClamp.angularOffset = 0f;
            _activeClamp.maxForce = Master.Object.GripStrength.x;
            _activeClamp.maxTorque = 0f;
            _activeClamp.correctionScale = Master.Object.GripStrength.y;

            // Set the indicator ring to the player color and scale
            gIndicatorRing.LeanScale(Vector2.one * IndicatorScale, AnimTime)
                .setEaseInOutCubic();
            gIndicatorRing.LeanColor(_activePlayer.Master.Object.Color.SetAlpha(.25f), AnimTime)
                .setEaseInOutCubic();
        }

        public void UngrabPlayer()
        {
            // Reset the logic output
            input = Vector2.zero;

            // Eject the player from the mount
            _activePlayer.autoSendInput = true;
            Destroy(_activeClamp);
            _activeClamp = null;
            _activeRigidbody.AddForce(MathfExt.ToVector(Master.Object.EjectionForce.x * Mathf.Deg2Rad,
                Master.Object.EjectionForce.y), ForceMode2D.Impulse);

            // Reset the tracker variables
            active = false;
            _activePlayerId = "";
            _activePlayer = null;
            _activeRigidbody = null;

            // Reset the indicator ring
            gIndicatorRing.LeanScale(Vector2.one, AnimTime)
                .setEaseInOutCubic();
            gIndicatorRing.LeanColor(new Color(1f, 1f, 1f, .1f), AnimTime)
                .setEaseInOutCubic();

            // Temporarily lock the mount
            _cooldown = true;
            gameObject.LeanDelayedCall(CooldownTime, () =>
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
            if (!active) return;
            UngrabPlayer();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Grab any players that come in contact
            if (active || _cooldown || !other.CompareTag(Tags.PlayerTag)) return;
            _activePlayer = other.GetComponent<PlayerController>().GetSub<PlayerInputController>("input");
            if (_activePlayer == null) throw Errors.NoItemFound("player subcontroller", "input");
            GrabPlayer(_activePlayer);
        }

        private void FixedUpdate()
        {
            // Send player movement input to public variable for use in the logic system
            if (!active || _cooldown) return;
            input = _activePlayer.movementInput;

            // Ungrab the active player if the boost input is triggered
            if (!_activePlayer.boostInput) return;
            UngrabPlayer();
        }
    }
}