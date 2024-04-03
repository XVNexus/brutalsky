using UnityEngine;
using Utils.Ext;

namespace Core
{
    public class CameraSystem : BsBehavior
    {
        // Static instance
        public static CameraSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Local variables
        public Vector2 viewSize;
        public float dampening;
        public float shakeInterval;
        public float simSpeed;
        private float _offsetMultiplier;
        private Vector2 _offset;
        private Vector2 _velocity;
        private float _shake;
        private float _shakeTimer;
        private float _lastCameraAspect;

        // External references
        private Camera _cCamera;

        // Init functions
        protected override void OnStart()
        {
            _cCamera = Camera.main;
        }

        // System functions
        public void ResizeView(Vector2 viewSize)
        {
            this.viewSize = viewSize;
            _offsetMultiplier = Mathf.Min(viewSize.x, viewSize.y);
            _cCamera.orthographicSize =
                Mathf.Max(viewSize.y * .5f * (viewSize.x / viewSize.y) / _cCamera.aspect, viewSize.y * .5f);
        }

        public void Shove(Vector2 force)
        {
            _velocity += force * .01f;
        }

        public void Shake(float force)
        {
            _shake += force;
            _shakeTimer = shakeInterval;
        }

        // Event functions
        private void Update()
        {
            // Configure the viewport to fit the screen
            if (!Mathf.Approximately(_cCamera.aspect, _lastCameraAspect))
            {
                ResizeView(viewSize);
                _lastCameraAspect = _cCamera.aspect;
            }

            // Simulate camera spring mount
            var velocityFromSpring = _velocity + -_offset * (simSpeed * Time.deltaTime);
            var velocityFromDampening = -_offset;
            _velocity = MathfExt.Lerp(velocityFromSpring, velocityFromDampening, dampening);
            _offset += _velocity * (simSpeed * Time.deltaTime);

            // Apply shake offset
            var cameraTransform = _cCamera.transform;
            var scaledOffset = _offset * _offsetMultiplier;
            cameraTransform.position = new Vector3(scaledOffset.x, scaledOffset.y, cameraTransform.position.z);
        }

        private void FixedUpdate()
        {
            // Convert shake to shove
            _shakeTimer += Time.fixedDeltaTime;
            if (_shakeTimer < shakeInterval) return;
            Shove(EventSystem.Random.NextFloat2Direction() * _shake);
            _shake -= _shake * simSpeed * 2f * Time.fixedDeltaTime;
            _shakeTimer -= shakeInterval;
        }
    }
}
