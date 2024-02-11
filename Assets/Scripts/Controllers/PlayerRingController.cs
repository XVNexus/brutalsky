using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerRingController : MonoBehaviour
    {
        public SpriteRenderer cSpriteRenderer;
        public SpriteMask cSpriteMask;

        private PlayerHealthController cPlayerHealthController;
        private PlayerMovementController cPlayerMovementController;
        private float ringAlpha = 0f;
        private float ringThickness = 0f;
        private float ringSpin = 0f;

        private void Start()
        {
            cPlayerHealthController = GetComponent<PlayerHealthController>();
            cPlayerMovementController = GetComponent<PlayerMovementController>();
        }

        private void FixedUpdate()
        {
            float targetRingAlpha;
            float targetRingThickness = cPlayerHealthController.health / cPlayerHealthController.maxHealth;
            float targetRingSpin;
            if (cPlayerMovementController.boostCharge > 0f)
            {
                targetRingAlpha = .25f + cPlayerMovementController.boostCharge * .25f;
                targetRingSpin = (Mathf.Pow(cPlayerMovementController.boostCharge, 2f) + 2f) * 180f + 90f;
            }
            else if (cPlayerMovementController.boostCooldown > 0f)
            {
                targetRingAlpha = .05f;
                targetRingSpin = 10f;
            }
            else
            {
                targetRingAlpha = .25f;
                targetRingSpin = 40f;
            }

            ringAlpha = MathfExt.MoveTo(ringAlpha, targetRingAlpha, Time.fixedDeltaTime);
            ringThickness = MathfExt.MoveTo(ringThickness, targetRingThickness, Time.fixedDeltaTime);
            ringSpin = MathfExt.MoveTo(ringSpin, targetRingSpin, 1440f * Time.fixedDeltaTime);

            var ringColor = cSpriteRenderer.color;
            ringColor.a = ringAlpha;
            cSpriteRenderer.color = ringColor;
            var ringTransform = cSpriteRenderer.transform;
            var ringAngles = ringTransform.localEulerAngles;
            ringAngles.z -= ringSpin * Time.fixedDeltaTime;
            ringTransform.localEulerAngles = ringAngles;
            var ringMaskScale = 2f - ringThickness * .8f;
            cSpriteMask.transform.localScale = new Vector3(ringMaskScale, ringMaskScale, 1f);
        }
    }
}
