using UnityEngine;
using Utils;
using Random = Unity.Mathematics.Random;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        public Vector2 viewSize = new(20f, 10f);
        public Vector2 offset;
        public Vector2 velocity;
        public float shake;
        public float dampening = .1f;
        public float simSpeed = 10f;
        public float shakeTimer;
        public float shakeInterval = .25f;
        public float lastCameraAspect = 0f;
        public Random random = Random.CreateFromIndex(0);

        private Camera cCamera;

        public void Shove(Vector2 force)
        {
            velocity += force;
        }

        public void Shake(float force)
        {
            shake += force;
            shakeTimer = shakeInterval;
        }

        private void OnCameraShake(Vector2 shove, float shake)
        {
            Shove(shove);
            Shake(shake);
        }

        private void Start()
        {
            EventSystem.current.OnCameraShake += OnCameraShake;
            cCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            // Configure the viewport to fit the screen
            if (!Mathf.Approximately(cCamera.aspect, lastCameraAspect))
            {
                cCamera.orthographicSize =
                    Mathf.Max(viewSize.y * (viewSize.x / viewSize.y) / cCamera.aspect, viewSize.y);
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
