using UnityEngine;
using Utils;
using Utils.Ext;
using Random = Unity.Mathematics.Random;

namespace Core
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem current;
        private void Awake() => current = this;

        // Variables
        public Vector2 viewSize = new(10f, 10f);
        public float dampening = .1f;
        public float shakeInterval = .1f;
        public float simSpeed = 10f;
        private float offsetMultiplier;
        private Vector2 offset;
        private Vector2 velocity;
        private float shake;
        private float shakeTimer;
        private float lastCameraAspect = 0f;
        private Random random = Random.CreateFromIndex(0);

        // References
        private Camera cCamera;

        // Functions
        public void ResizeView(Vector2 viewSize)
        {
            this.viewSize = viewSize;
            offsetMultiplier = Mathf.Min(viewSize.x, viewSize.y);
        }

        public void Shove(Vector2 force)
        {
            velocity += force * .01f;
        }

        public void Shake(float force)
        {
            shake += force;
            shakeTimer = shakeInterval;
        }

        // Events
        private void Start()
        {
            cCamera = Camera.main;
        }

        // Updates
        private void Update()
        {
            // Configure the viewport to fit the screen
            if (!Mathf.Approximately(cCamera.aspect, lastCameraAspect))
            {
                cCamera.orthographicSize =
                    Mathf.Max(viewSize.y * .5f * (viewSize.x / viewSize.y) / cCamera.aspect, viewSize.y * .5f);
                lastCameraAspect = cCamera.aspect;
            }

            // Simulate camera spring mount
            var velocityFromSpring = velocity + -offset * (simSpeed * Time.deltaTime);
            var velocityFromDampening = -offset;
            velocity = MathfExt.Lerp(velocityFromSpring, velocityFromDampening, dampening);
            offset += velocity * (simSpeed * Time.deltaTime);

            // Apply shake offset
            var cameraTransform = cCamera.transform;
            var scaledOffset = offset * offsetMultiplier;
            cameraTransform.position = new Vector3(scaledOffset.x, scaledOffset.y, cameraTransform.position.z);
        }

        private void FixedUpdate()
        {
            // Convert shake to shove
            shakeTimer += Time.fixedDeltaTime;
            if (shakeTimer < shakeInterval) return;
            Shove(random.NextFloat2Direction() * shake);
            shake -= shake * simSpeed * 2f * Time.fixedDeltaTime;
            shakeTimer -= shakeInterval;
        }
    }
}
