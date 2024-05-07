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
        public const float TransitionTime = .25f;
        public const float IndicatorScale = 2.2f / .8f / 1.5f;

        // Local variables
        public bool active;
        public PlayerInputController activePlayer;
        public Vector2 input;
        private string _activePlayerId = "";
        private bool _transitioning;

        // External references
        public GameObject gIndicatorRing;
        public Collider2D[] cClampCollider2Ds;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnMapCleanup += OnMapCleanup;
        }

        private void OnDestroy()
        {
            EventSystem._.OnMapCleanup -= OnMapCleanup;
        }

        // System functions
        public void GrabPlayer(PlayerInputController player)
        {
            active = true;
            _activePlayerId = player.Master.Object.Id;
            _transitioning = true;
            player.autoSendInput = false;
            MapSystem._.SetPlayerFrozen(player.gameObject, true, true);
            player.gameObject.LeanMoveLocal(transform.position, TransitionTime)
                .setEaseOutCubic()
                .setOnComplete(() =>
                {
                    foreach (var clamp in cClampCollider2Ds)
                    {
                        clamp.enabled = true;
                    }
                    MapSystem._.SetPlayerFrozen(player.gameObject, false);
                    _transitioning = false;
                });
            gIndicatorRing.LeanScale(Vector2.one * IndicatorScale, TransitionTime)
                .setEaseInOutCubic();
            gIndicatorRing.LeanColor(new Color(1f, 1f, 1f, .5f), TransitionTime)
                .setEaseInOutCubic();
        }

        public void UngrabPlayer(bool forceExit = true)
        {
            input = Vector2.zero;
            active = false;
            _activePlayerId = "";
            foreach (var clamp in cClampCollider2Ds)
            {
                clamp.enabled = false;
            }
            activePlayer.gameObject.LeanRotateZ(0f, TransitionTime)
                .setEaseOutCubic();
            if (forceExit)
            {
                _transitioning = true;
                MapSystem._.SetPlayerFrozen(activePlayer.gameObject, true);
                var exitOffset = MathfExt.ToVector(Master.Object.ExitAngle * Mathf.Deg2Rad, 1.5f);
                activePlayer.gameObject.LeanMoveLocal(transform.position + (Vector3)exitOffset, TransitionTime)
                    .setEaseOutCubic()
                    .setOnComplete(() =>
                    {
                        MapSystem._.SetPlayerFrozen(activePlayer.gameObject, false);
                        activePlayer.autoSendInput = true;
                        activePlayer = null;
                        _transitioning = false;
                    });
            }
            else
            {
                activePlayer.autoSendInput = true;
                activePlayer = null;
            }
            gIndicatorRing.LeanScale(Vector2.one, TransitionTime)
                .setEaseInOutCubic();
            gIndicatorRing.LeanColor(new Color(1f, 1f, 1f, .1f), TransitionTime)
                .setEaseInOutCubic();
        }

        // Event functions
        private void OnMapCleanup(BsMap map)
        {
            // Kick out any mounted players before the map is unloaded
            if (!active) return;
            activePlayer.autoSendInput = true;
            active = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Grab any players that come in contact
            if (active || _transitioning || !other.CompareTag(Tags.PlayerTag)) return;
            activePlayer = other.GetComponent<PlayerController>().GetSub<PlayerInputController>("input");
            if (activePlayer == null) throw Errors.NoItemFound("player subcontroller", "input");
            GrabPlayer(activePlayer);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Ungrab the active player if it clips out of the clamps
            if (active && !_transitioning && other.CompareTag(Tags.PlayerTag)
                && other.GetComponent<PlayerController>().Object.Id == _activePlayerId
                && !MapSystem._.GetPlayerFrozen(activePlayer.gameObject))
            {
                UngrabPlayer(false);
            }
        }

        private void FixedUpdate()
        {
            // Send player movement input to public variable for use in the logic system
            if (!active || _transitioning) return;
            input = activePlayer.movementInput;

            // Ungrab the active player if the boost input is triggered
            if (!activePlayer.boostInput) return;
            UngrabPlayer();
        }
    }
}
