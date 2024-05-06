using System;
using Brutalsky.Object;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    public class PlayerInputController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "input";
        public override bool IsUnused => false;

        // Local variables
        public bool autoSendInput = true;
        public Vector2 movementInput;
        public bool boostInput;

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
        }

        protected override void OnLink()
        {
            _mMovement = Master.RequireSub<PlayerMovementController>("movement", Id);
        }

        // Event functions
        private void FixedUpdate()
        {
            if (Status < Ready) return;
            movementInput = _iMovement.ReadValue<Vector2>();
            boostInput = _iBoost.IsPressed();
            if (!autoSendInput) return;
            _mMovement.SendInput(movementInput, boostInput);
        }
    }
}
