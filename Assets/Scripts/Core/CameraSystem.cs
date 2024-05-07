using Controllers.Base;
using UnityEngine;
using Utils.Ext;

namespace Core
{
    public class CameraSystem : BsBehavior
    {
        // Static instance
        public static CameraSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Config options
        public float dampening;
        public float shakeInterval;
        public float simSpeed;

        // Exposed properties
        public Rect ViewRect { get; private set; }
        public Vector2 TrackPoint { get; private set; }

        // Local variables
        private float _offsetMultiplier;
        private Vector2 _shoveOffset;
        private Vector2 _shoveVelocity;
        private float _shake;
        private float _shakeTimer;
        private float _lastCameraAspect;

        // External references
        public GameObject gCameraMount;
        public SpriteRenderer cCameraCover;
        private Camera _cCamera;

        // Init functions
        protected override void OnStart()
        {
            _cCamera = Camera.main;

            InvokeRepeating(nameof(RareUpdate), 1f, 1f);
        }

        // System functions
        public void Resize(Rect viewRect)
        {
            ViewRect = viewRect;
            TrackPoint = viewRect.center;
            _offsetMultiplier = Mathf.Min(viewRect.width, viewRect.height);
            _cCamera.orthographicSize = viewRect.ForceAspect(_cCamera.aspect).height * .5f;
        }

        public void Shove(Vector2 force)
        {
            _shoveVelocity += force * .01f;
        }

        public void Shake(float force)
        {
            _shake += force;
            _shakeTimer = shakeInterval;
        }

        // Event functions
        private void FixedUpdate()
        {
            // Convert shake to shove
            _shakeTimer += Time.fixedDeltaTime;
            if (_shakeTimer >= shakeInterval)
            {
                Shove(ResourceSystem.Random.NextFloat2Direction() * _shake);
                _shake -= _shake * simSpeed * 2f * Time.fixedDeltaTime;
                _shakeTimer -= shakeInterval;
            }

            // Simulate camera spring mount
            var velocityFromSpring = _shoveVelocity + -_shoveOffset * (simSpeed * Time.fixedDeltaTime);
            var velocityFromDampening = -_shoveOffset;
            _shoveVelocity = MathfExt.Lerp(velocityFromSpring, velocityFromDampening, dampening);
            _shoveOffset += _shoveVelocity * (simSpeed * Time.fixedDeltaTime);

            // Apply shove offset
            var cameraTransform = _cCamera.transform;
            var scaledOffset = _shoveOffset * _offsetMultiplier;
            var newPosition = TrackPoint + scaledOffset;
            cameraTransform.localPosition = new Vector3(newPosition.x, newPosition.y, cameraTransform.localPosition.z);
        }

        private void RareUpdate()
        {
            // Configure the viewport to fit the screen
            if (Mathf.Approximately(_cCamera.aspect, _lastCameraAspect)) return;
            _cCamera.orthographicSize = Mathf.Max(
                ViewRect.height * .5f * ViewRect.Aspect() / _cCamera.aspect, ViewRect.height * .5f);
            _lastCameraAspect = _cCamera.aspect;
        }
    }
}
