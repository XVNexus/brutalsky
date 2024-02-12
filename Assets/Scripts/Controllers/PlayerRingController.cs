using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerRingController : MonoBehaviour
    {
        // Variables
        private float ringAlpha;
        private float ringThickness;
        private float ringSpin;

        // References
        public SpriteRenderer cSpriteRenderer;
        public SpriteMask cSpriteMask;
        private OptionalComponent<PlayerHealthController> cPlayerHealthController;
        private OptionalComponent<PlayerMovementController> cPlayerMovementController;

        // Events
        private void Start()
        {
            cPlayerHealthController =
                new OptionalComponent<PlayerHealthController>(GetComponent<PlayerHealthController>());
            cPlayerMovementController =
                new OptionalComponent<PlayerMovementController>(GetComponent<PlayerMovementController>());
        }

        // Updates
        private void Update()
        {
            // Calculate target ring properties
            var targetRingAlpha = .25f;
            var targetRingThickness = cPlayerHealthController.exists
                ? cPlayerHealthController.component.health / cPlayerHealthController.component.maxHealth
                : 1f;
            var targetRingSpin = 40f;
            if (cPlayerMovementController.exists)
            {
                if (cPlayerMovementController.component.boostCharge > 0f)
                {
                    targetRingAlpha = .25f + cPlayerMovementController.component.boostCharge * .25f;
                    targetRingSpin = (Mathf.Pow(cPlayerMovementController.component.boostCharge, 2f) + 2f) * 180f + 90f;
                }
                else if (cPlayerMovementController.component.boostCooldown > 0f)
                {
                    targetRingAlpha = .05f;
                    targetRingSpin = 10f;
                }
                else
                {
                    targetRingAlpha = .25f;
                    targetRingSpin = 40f;
                }
            }

            // Transition current ring properties to calculated target properties
            ringAlpha = MathfExt.MoveTo(ringAlpha, targetRingAlpha, Time.deltaTime);
            ringThickness = MathfExt.MoveTo(ringThickness, targetRingThickness, Time.deltaTime);
            ringSpin = MathfExt.MoveTo(ringSpin, targetRingSpin, 1440f * Time.deltaTime);

            // Apply current ring properties
            var ringColor = cSpriteRenderer.color;
            ringColor.a = ringAlpha;
            cSpriteRenderer.color = ringColor;
            var ringTransform = cSpriteRenderer.transform;
            var ringAngles = ringTransform.localEulerAngles;
            ringAngles.z -= ringSpin * Time.deltaTime;
            ringTransform.localEulerAngles = ringAngles;
            var ringMaskScale = 2f - ringThickness * .8f;
            cSpriteMask.transform.localScale = new Vector3(ringMaskScale, ringMaskScale, 1f);
        }
    }
}
