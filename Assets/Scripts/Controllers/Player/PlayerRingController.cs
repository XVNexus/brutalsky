using UnityEngine;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerRingController : MonoBehaviour
    {
        // Variables
        private float _ringAlpha;
        private float _ringThickness;
        private float _ringSpin;

        // References
        public SpriteRenderer cSpriteRenderer;
        public SpriteMask cSpriteMask;
        private PlayerController _cPlayerController;

        // Events
        private void Start()
        {
            _cPlayerController = GetComponent<PlayerController>();
        }

        // Updates
        private void Update()
        {
            // Calculate target ring properties
            var targetRingAlpha = .25f;
            var targetRingThickness = _cPlayerController.health / _cPlayerController.maxHealth;
            var targetRingSpin = 40f;
            if (_cPlayerController.boostCharge > 0f)
            {
                targetRingAlpha = .25f + _cPlayerController.boostCharge * .25f;
                targetRingSpin = (Mathf.Pow(_cPlayerController.boostCharge, 1.5f) + 1.5f) * 360f;
            }
            else if (_cPlayerController.boostCooldown > 0f)
            {
                targetRingAlpha = .05f;
                targetRingSpin = 10f;
            }

            // Transition current ring properties to calculated target properties
            _ringAlpha = MathfExt.MoveTo(_ringAlpha, targetRingAlpha, Time.deltaTime);
            _ringThickness = MathfExt.MoveTo(_ringThickness, targetRingThickness, Time.deltaTime);
            _ringSpin = MathfExt.MoveTo(_ringSpin, targetRingSpin, 1440f * Time.deltaTime);

            // Apply current ring properties
            var ringColor = cSpriteRenderer.color;
            ringColor.a = _ringAlpha;
            cSpriteRenderer.color = ringColor;
            var ringTransform = cSpriteRenderer.transform;
            var ringAngles = ringTransform.localEulerAngles;
            ringAngles.z -= _ringSpin * Time.deltaTime;
            ringTransform.localEulerAngles = ringAngles;
            var ringMaskScale = 1.8f - _ringThickness * .6f;
            cSpriteMask.transform.localScale = new Vector3(ringMaskScale, ringMaskScale, 1f);
        }
    }
}
