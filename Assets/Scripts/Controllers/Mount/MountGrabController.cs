using Brutalsky.Object;
using Controllers.Base;
using Controllers.Player;
using Core;
using UnityEngine;
using Utils.Constants;

namespace Controllers.Mount
{
    public class MountGrabController : SubControllerBase<BsMount>
    {
        // Controller metadata
        public override string Id => "grab";
        public override bool IsUnused => !Master.Object.Simulated;

        // Local constants
        public const float TransitionTime = .25f;

        // Local variables
        public bool active;
        public PlayerInputController player;
        public Vector2 input;
        private Transform _originalPlayerParent;
        private bool _transitioning;

        // Event functions
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Grab any player that comes in contact with the mount
            if (active || _transitioning || !other.CompareTag(Tags.PlayerTag)) return;
            player = other.GetComponent<PlayerController>().GetSub<PlayerInputController>("input");
            if (player == null) throw Errors.NoItemFound("player subcontroller", "input");
            active = true;
            player.autoSendInput = false;
            MapSystem._.SetPlayerLocked(player.gameObject, true, true);
            _originalPlayerParent = player.transform.parent;
            player.transform.parent = transform;
            _transitioning = true;
            player.gameObject.LeanMoveLocal(Vector2.zero, TransitionTime)
                .setEaseOutCubic()
                .setOnComplete(() =>
                {
                    _transitioning = false;
                });
        }

        private void FixedUpdate()
        {
            // Send player movement input to public variable for use in the logic system
            if (!active || _transitioning) return;
            input = player.movementInput;

            // Unmount player when boost input is received
            if (!player.boostInput) return;
            input = Vector2.zero;
            player.transform.parent = _originalPlayerParent;
            _originalPlayerParent = null;
            _transitioning = true;
            player.gameObject.LeanMoveLocal(transform.position + (Vector3)Master.Object.Exit, TransitionTime)
                .setEaseOutCubic()
                .setOnComplete(() =>
                {
                    MapSystem._.SetPlayerLocked(player.gameObject, false);
                    player.autoSendInput = true;
                    active = false;
                    player = null;
                    _transitioning = false;
                });
            player.gameObject.LeanRotateZ(0f, TransitionTime)
                .setEaseOutCubic();
        }
    }
}
