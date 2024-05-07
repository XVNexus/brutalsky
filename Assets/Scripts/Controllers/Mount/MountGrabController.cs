using System;
using Brutalsky;
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
        public PlayerInputController activePlayer;
        public Vector2 input;
        private string _activePlayerId = "";
        private bool _transitioning;
        private SpriteRenderer _cSpriteRenderer;

        // External references
        public Collider2D[] _cClampCollider2Ds;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnMapCleanup += OnMapCleanup;

            _cSpriteRenderer = GetComponent<SpriteRenderer>();
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
                    foreach (var clamp in _cClampCollider2Ds)
                    {
                        clamp.enabled = true;
                    }
                    MapSystem._.SetPlayerFrozen(player.gameObject, false);
                    _transitioning = false;
                });
        }

        public void UngrabPlayer()
        {
            input = Vector2.zero;
            active = false;
            _activePlayerId = "";
            _transitioning = true;
            foreach (var clamp in _cClampCollider2Ds)
            {
                clamp.enabled = false;
            }
            MapSystem._.SetPlayerFrozen(activePlayer.gameObject, true);
            activePlayer.gameObject.LeanMoveLocal(transform.position + (Vector3)Master.Object.Exit, TransitionTime)
                .setEaseOutCubic()
                .setOnComplete(() =>
                {
                    MapSystem._.SetPlayerFrozen(activePlayer.gameObject, false);
                    activePlayer.autoSendInput = true;
                    activePlayer = null;
                    _transitioning = false;
                });
            activePlayer.gameObject.LeanRotateZ(0f, TransitionTime)
                .setEaseOutCubic();
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
                UngrabPlayer();
            }
        }

        private void Update()
        {
            _cSpriteRenderer.color = new Color(_transitioning ? 1f : 0f, active ? 1f : 0f, 0f);
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
