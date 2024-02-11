using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerRingController : MonoBehaviour
    {
        public float idleSpinSpeed = 30f;
        public float chargeSpinSpeed = 180f;
        public SpriteRenderer cSpriteRenderer;
        public SpriteMask cSpriteMask;

        private PlayerHealthController cPlayerHealthController;
        private PlayerMovementController cPlayerMovementController;
        private float ringHue = 0f;
        private float ringAlpha = 0f;
        private float ringSpinSpeed = 0f;
        private float ringMask = 0f;

        private void Start()
        {
            cPlayerHealthController = GetComponent<PlayerHealthController>();
            cPlayerMovementController = GetComponent<PlayerMovementController>();
        }

        private void FixedUpdate()
        {
            var targetRingHue = (cPlayerHealthController.health / cPlayerHealthController.maxHealth) / 3f;
            float targetRingAlpha;
            float targetRingSpinSpeed;
            float targetRingMask;
            if (cPlayerMovementController.boostCharge > 0f)
            {
                targetRingAlpha = .25f + cPlayerMovementController.boostCharge * .75f / 3f;
                targetRingSpinSpeed = (Mathf.Pow(cPlayerMovementController.boostCharge, 2f) + 2f) * chargeSpinSpeed + 90f;
                targetRingMask = .5f + cPlayerMovementController.boostCharge * .5f / 3f;
            }
            else if (cPlayerMovementController.boostCooldown > 0f)
            {
                targetRingAlpha = .25f;
                targetRingSpinSpeed = 0f;
                targetRingMask = .25f;
            }
            else
            {
                targetRingAlpha = .4f;
                targetRingSpinSpeed = idleSpinSpeed;
                targetRingMask = .5f;
            }

            ringHue = MathfExt.MoveTo(ringHue, targetRingHue, Time.fixedDeltaTime);
            ringAlpha = MathfExt.MoveTo(ringAlpha, targetRingAlpha, Time.fixedDeltaTime);
            ringSpinSpeed = MathfExt.MoveTo(ringSpinSpeed, targetRingSpinSpeed, 1440f * Time.fixedDeltaTime);
            ringMask = MathfExt.MoveTo(ringMask, targetRingMask, Time.fixedDeltaTime);

            var ringColorRgb = Color.HSVToRGB(ringHue, .75f, 1f);
            cSpriteRenderer.color = new Color(ringColorRgb.r, ringColorRgb.g, ringColorRgb.b, ringAlpha);
            var ringTransform = cSpriteRenderer.transform;
            var ringAngles = ringTransform.localEulerAngles;
            ringAngles.z -= ringSpinSpeed * Time.fixedDeltaTime;
            ringTransform.localEulerAngles = ringAngles;
            var ringMaskScale = 2f - ringMask * .8f;
            cSpriteMask.transform.localScale = new Vector3(ringMaskScale, ringMaskScale, 1f);
        }
    }
}
