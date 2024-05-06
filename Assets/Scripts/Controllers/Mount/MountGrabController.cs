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

        // Event functions
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Grab any player that comes in contact with the mount
            if (active || !other.CompareTag(Tags.PlayerTag)) return;
            player = other.GetComponent<PlayerController>().GetSub<PlayerInputController>("input");
            if (player == null) throw Errors.NoItemFound("player subcontroller", "input");
            active = true;
            player.autoSendInput = false;
            MapSystem._.SetPlayerFrozen(player.gameObject, true, true);
            player.gameObject.LeanMoveLocal(transform.localPosition, TransitionTime)
                .setEaseOutCubic();
        }

        private void FixedUpdate()
        {
            // Send player movement input to public variable for use in the logic system
            if (!active) return;
            input = player.movementInput;

            // Unmount player when boost input is received
            if (!player.boostInput) return;
            active = false;
            input = Vector2.zero;
            player.gameObject.LeanMoveLocal(transform.localPosition + (Vector3)Master.Object.Exit, TransitionTime)
                .setEaseOutCubic()
                .setOnComplete(() =>
                {
                    MapSystem._.SetPlayerFrozen(player.gameObject, false);
                    player.autoSendInput = true;
                });
        }
    }
}
