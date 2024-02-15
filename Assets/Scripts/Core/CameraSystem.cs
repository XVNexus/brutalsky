using UnityEngine;
using Utils;
using Random = Unity.Mathematics.Random;

namespace Core
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem current;
        private void Awake() => current = this;

        // Settings
        public Vector2 viewSize = new(40f, 20f);
        public float dampening = .1f;
        public float shakeInterval = .1f;
        public float simSpeed = 10f;

        // Variables
        private Vector2 offset;
        private Vector2 velocity;
        private float shake;
        private float shakeTimer;
        private float lastCameraAspect = 0f;
        private Random random = Random.CreateFromIndex(0);

        // References
        private Camera cCamera;

        // Functions
        public void Shove(Vector2 force)
        {
            velocity += force;
        }

        public void Shake(float force)
        {
            shake += force;
            shakeTimer = shakeInterval;
        }

        // Events
        private void Start()
        {
            EventSystem.current.OnCameraShake += OnCameraShake;
            cCamera = Camera.main;
        }

        private void OnCameraShake(Vector2 shove, float shake)
        {
            Shove(shove);
            Shake(shake);
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
            velocity = MathfExt.Lerp2(velocityFromSpring, velocityFromDampening, dampening);
            offset += velocity * (simSpeed * Time.deltaTime);

            // Apply shake offset
            var cameraTransform = cCamera.transform;
            cameraTransform.position = new Vector3(offset.x, offset.y, cameraTransform.position.z);
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
