using Controllers.Base;
using Data.Object;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;

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

        // Local variables
        private Vector2 _lastMovementInput;
        private bool _lastBoostInput;

        // External references
        private InputAction _iMovement;
        private InputAction _iBoost;

        // Linked modules
        private PlayerMovementController _mMovement;

        // Init functions
        protected override void OnInit()
        {
            var inputSetId = Master.Object.Type switch
            {
                BsPlayer.TypeMain => "Player Main",
                BsPlayer.TypeLocal1 => "Player Local1",
                BsPlayer.TypeLocal2 => "Player Local2",
                _ => "Player Main"
            };
            _iMovement = EventSystem._.GetInputAction(inputSetId, "Movement");
            _iBoost = EventSystem._.GetInputAction(inputSetId, "Boost");
            Dummy = Master.Object.Type == BsPlayer.TypeDummy;
        }

        protected override void OnLink()
        {
            _mMovement = Master.RequireSub<PlayerMovementController>("movement", Id);
        }

        // Event functions
        private void FixedUpdate()
        {
            // Get input data
            if (Dummy || Status < Ready) return;
            // The _last buffers are used to cover up frame gaps when using multiple keyboards
            var movementInput = _iMovement.ReadValue<Vector2>();
            MovementInput = movementInput.magnitude != 0f || _lastMovementInput.magnitude == 0f
                ? movementInput
                : _lastMovementInput;
            _lastMovementInput = movementInput;
            var boostInput = _iBoost.ReadValue<float>() > 0f;
            BoostInput = boostInput || !_lastBoostInput
                ? boostInput
                : _lastBoostInput;
            _lastBoostInput = boostInput;

            // Send input data to movement controller
            if (autoSendInput)
            {
                _mMovement.SendInput(MovementInput, BoostInput);
            }
        }
    }
}
