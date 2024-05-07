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

        // Local variables
        public Vector2 movementInput;
        public bool boostInput;
        public bool autoSendInput = true;
        public bool blockInput;
        public bool dummy;

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
            dummy = Master.Object.Type == PlayerType.Dummy;
        }

        protected override void OnLink()
        {
            _mMovement = Master.RequireSub<PlayerMovementController>("movement", Id);
        }

        // Event functions
        private void FixedUpdate()
        {
            if (dummy) return;
            if (!blockInput)
            {
                if (Status < Ready) return;
                movementInput = _iMovement.ReadValue<Vector2>();
                boostInput = _iBoost.IsPressed();
                if (!autoSendInput) return;
                _mMovement.SendInput(movementInput, boostInput);
            }
            else
            {
                movementInput = Vector2.zero;
                boostInput = false;
            }
        }
    }
}
