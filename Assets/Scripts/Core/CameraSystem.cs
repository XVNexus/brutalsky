using System.Collections.Generic;
using System.Linq;
using Brutalsky;
using Brutalsky.Base;
using Brutalsky.Object;
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
        public float shoveDampening;
        public float shakeInterval;
        public float simSpeed;
        public Vector2 followSpeed;
        public float followRectScale;
        public float followMinSize;
        public float followViewSizeThreshold;

        // Exposed properties
        public Rect BaseRect { get; private set; } = new(-20f, -10f, 40f, 20f);
        public Vector2 ViewPosition { get; private set; } = new(0f, 0f);
        public Vector2 ViewSize { get; private set; } = new(40f, 20f);
        public float OrthoSize { get; private set; } = 10f;
        public Dictionary<string, Transform> FollowTargets { get; } = new();

        // Local variables
        private Vector2 _shoveOffset;
        private Vector2 _shoveVelocity;
        private float _shake;
        private float _shakeTimer;
        private float _lastCameraAspect;
        private bool _enableFollow;
        private bool _isFollowing;
        private Rect _followRect;

        // External references
        public GameObject gCameraMount;
        public SpriteRenderer cCameraCover;
        private Camera _cCamera;

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnPlayerSpawn += OnPlayerSpawn;
            EventSystem._.OnPlayerDie += OnPlayerDie;

            _cCamera = Camera.main;
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
            EventSystem._.OnPlayerDie -= OnPlayerDie;
        }

        // System functions
        public void AddShove(Vector2 force)
        {
            _shoveVelocity += force * .01f;
        }

        public void AddShake(float force)
        {
            _shake += force;
            _shakeTimer = shakeInterval;
        }

        public void StartFollowing(BsObject obj)
        {
            FollowTargets[obj.Id] = obj.InstanceObject.transform;
            _isFollowing = true;
        }

        public void StopFollowing(BsObject obj)
        {
            StopFollowing(obj.Id);
        }

        public void StopFollowing(string id)
        {
            FollowTargets.Remove(id);
            _isFollowing = FollowTargets.Count > 0;
        }

        public void ClearFollowing()
        {
            FollowTargets.Clear();
            _isFollowing = false;
        }

        public void SetBaseRect(Rect baseRect)
        {
            BaseRect = baseRect;
            _enableFollow = baseRect.size.magnitude > followViewSizeThreshold;
            SetViewRect(baseRect);
        }

        public void SetViewRect(Rect viewRect)
        {
            ViewPosition = viewRect.center;
            ViewSize = viewRect.size;
            _cCamera.orthographicSize = ViewSize.ForceAspect(_cCamera.aspect).y * .5f;
        }

        public void SetViewRectSmooth(Rect viewRect, float positionFactor, float sizeFactor)
        {
            ViewPosition = MathfExt.MoveToExponential(ViewPosition, viewRect.center, positionFactor);
            ViewSize = viewRect.size;
            _cCamera.orthographicSize = MathfExt.MoveToExponential(_cCamera.orthographicSize,
                ViewSize.ForceAspect(_cCamera.aspect).y * .5f, sizeFactor);
        }

        // Event functions
        private void OnPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visible)
        {
            if (visible)
            {
                StartFollowing(player);
            }
            else
            {
                StopFollowing(player);
            }
        }

        private void OnPlayerDie(BsMap map, BsPlayer player)
        {
            StopFollowing(player);
        }

        private void FixedUpdate()
        {
            // Convert shake to shove
            _shakeTimer += Time.fixedDeltaTime;
            if (_shakeTimer >= shakeInterval)
            {
                AddShove(ResourceSystem.Random.NextFloat2Direction() * _shake);
                _shake -= _shake * simSpeed * 2f * Time.fixedDeltaTime;
                _shakeTimer -= shakeInterval;
            }

            // Simulate camera spring mount
            var velocityFromSpring = _shoveVelocity + -_shoveOffset * (simSpeed * Time.fixedDeltaTime);
            var velocityFromDampening = -_shoveOffset;
            _shoveVelocity = MathfExt.Lerp(velocityFromSpring, velocityFromDampening, shoveDampening);
            _shoveOffset += _shoveVelocity * (simSpeed * Time.fixedDeltaTime);

            // Apply position and shove
            var cameraTransform = _cCamera.transform;
            var newPosition = ViewPosition + _shoveOffset * _cCamera.orthographicSize;
            cameraTransform.localPosition = new Vector3(newPosition.x, newPosition.y, cameraTransform.localPosition.z);

            // Focus on follow targets
            if (!_enableFollow) return;
            Rect targetRect;
            if (_isFollowing)
            {
                var followPositions = FollowTargets.Values.Select(transform => (Vector2)transform.position).ToArray();
                var min = followPositions[0];
                var max = followPositions[0];
                for (var i = 1; i < FollowTargets.Count; i++)
                {
                    var target = followPositions[i];
                    min = MathfExt.Min(target, min);
                    max = MathfExt.Max(target, max);
                }
                var followPosition = MathfExt.Mean(min, max);
                var followSize = (max - min).magnitude * followRectScale;
                targetRect = new Rect(followPosition, Vector2.zero)
                    .Resize(Vector2.one * Mathf.Max(followSize, followMinSize));
            }
            else
            {
                targetRect = BaseRect;
            }
            SetViewRectSmooth(targetRect, followSpeed.x * Time.fixedDeltaTime, followSpeed.y * Time.fixedDeltaTime);
        }
    }
}
