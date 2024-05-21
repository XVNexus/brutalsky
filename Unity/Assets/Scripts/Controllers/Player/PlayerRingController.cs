using System;
using Brutalsky;
using Brutalsky.Object;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerRingController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "ring";
        public override bool IsUnused => false;

        // Local variables
        private float _ringAlpha;
        private float _ringThickness;
        private float _ringSpin;

        // External references
        public Light2D cLight2D;
        public SpriteRenderer cRingSpriteRenderer;
        public SpriteMask cSpriteMask;
        private SpriteRenderer _cPlayerSpriteRenderer;

        // Linked modules
        [CanBeNull] private PlayerMovementController _mMovement;
        [CanBeNull] private PlayerHealthController _mHealth;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnPlayerSpawn += OnPlayerSpawn;

            _cPlayerSpriteRenderer = GetComponent<SpriteRenderer>();

            // Sync ring color with player color
            cLight2D.color = _cPlayerSpriteRenderer.color;
            cRingSpriteRenderer.color = _cPlayerSpriteRenderer.color;
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
        }

        protected override void OnLink()
        {
            _mMovement = Master.GetSub<PlayerMovementController>("movement");
            _mHealth = Master.GetSub<PlayerHealthController>("health");
        }

        // Event functions
        private void OnPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visible)
        {
            if (player.Id != Master.Object.Id) return;
            _ringAlpha = 0f;
            _ringThickness = 0f;
            _ringSpin = 0f;
            cRingSpriteRenderer.transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            // Calculate target ring properties
            var targetRingThickness = _mHealth ? _mHealth.Health / _mHealth.MaxHealth : 1f;
            var targetRingAlpha = .25f;
            var targetRingSpin = 40f;
            if (_mMovement)
            {
                if (_mMovement.BoostCharge > 0f)
                {
                    targetRingAlpha = .25f + _mMovement.BoostCharge * .25f;
                    targetRingSpin = (Mathf.Pow(_mMovement.BoostCharge, 1.5f) + 1.5f) * 360f;
                }
                else if (_mMovement.BoostCooldown > 0f)
                {
                    targetRingAlpha = .05f;
                    targetRingSpin = 10f;
                }
            }

            // Transition current ring properties to calculated target properties
            _ringThickness = MathfExt.MoveToExponential(_ringThickness, targetRingThickness, 5f * Time.deltaTime);
            _ringAlpha = MathfExt.MoveToLinear(_ringAlpha, targetRingAlpha, Time.deltaTime);
            _ringSpin = MathfExt.MoveToLinear(_ringSpin, targetRingSpin, 1440f * Time.deltaTime);

            // Apply current ring properties
            cSpriteMask.transform.localScale = Vector2.one * (2f - _ringThickness * .8f);
            var ringColor = cRingSpriteRenderer.color;
            ringColor.a = _ringAlpha;
            cRingSpriteRenderer.color = ringColor;
            var ringTransform = cRingSpriteRenderer.transform;
            var ringAngles = ringTransform.localEulerAngles;
            ringAngles.z -= _ringSpin * Time.deltaTime;
            ringTransform.localEulerAngles = ringAngles;
        }
    }
}