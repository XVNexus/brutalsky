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

        protected override void OnLink()
        {
            _mMovement = Master.GetSub<PlayerMovementController>("movement");
            _mHealth = Master.GetSub<PlayerHealthController>("health");
        }

        // Event functions
        private void OnPlayerSpawn(BsMap map, BsPlayer player)
        {
            if (player.Id != Master.Object.Id) return;
            _ringAlpha = 0f;
            _ringThickness = 0f;
            _ringSpin = 0f;
            cRingSpriteRenderer.transform.rotation = Quaternion.identity;
        }

        private void Update()
        {
            // Calculate target ring properties
            var targetRingThickness = _mHealth ? _mHealth.health / _mHealth.maxHealth : 1f;
            var targetRingAlpha = .25f;
            var targetRingSpin = 40f;
            if (_mMovement)
            {
                if (_mMovement.boostCharge > 0f)
                {
                    targetRingAlpha = .25f + _mMovement.boostCharge * .25f;
                    targetRingSpin = (Mathf.Pow(_mMovement.boostCharge, 1.5f) + 1.5f) * 360f;
                }
                else if (_mMovement.boostCooldown > 0f)
                {
                    targetRingAlpha = .05f;
                    targetRingSpin = 10f;
                }
            }

            // Transition current ring properties to calculated target properties
            _ringThickness = MathfExt.MoveTo(_ringThickness, targetRingThickness, Time.deltaTime);
            _ringAlpha = MathfExt.MoveTo(_ringAlpha, targetRingAlpha, Time.deltaTime);
            _ringSpin = MathfExt.MoveTo(_ringSpin, targetRingSpin, 1440f * Time.deltaTime);

            // Apply current ring properties
            var ringMaskScale = 1.8f - _ringThickness * .6f;
            cSpriteMask.transform.localScale = new Vector3(ringMaskScale, ringMaskScale, 1f);
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
