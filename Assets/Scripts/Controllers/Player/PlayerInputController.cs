using Brutalsky.Object;
using Controllers.Base;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Player;

namespace Controllers.Player
{
    public class PlayerInputController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "input";
        public override bool IsUnused => false;

        // Config options
        public bool autoSendInput;

        // Exposed properties
        public Vector2 MovementInput { get; private set; }
        public bool BoostInput { get; private set; }
        public bool Dummy { get; private set; }

        // External references
        private InputAction _iMovement;
        private InputAction _iBoost;

        // Linked modules
        private PlayerMovementController _mMovement;

        // Init functions
        protected override void OnInit()
        {
            _iMovement = EventSystem._.GetInputAction("Movement");
            _iBoost = EventSystem._.GetInputAction("Boost");

            // Disable input if the player is a dummy
            Dummy = Master.Object.Type == PlayerType.Dummy;
        }

        protected override void OnLink()
        {
            _mMovement = Master.RequireSub<PlayerMovementController>("movement", Id);
        }

        // Event functions
        private void FixedUpdate()
        {
            if (Dummy || Status < Ready) return;
            MovementInput = _iMovement.ReadValue<Vector2>();
            BoostInput = _iBoost.IsPressed();
            if (autoSendInput)
            {
                _mMovement.SendInput(MovementInput, BoostInput);
            }
        }
    }
}
