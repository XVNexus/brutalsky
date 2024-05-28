using System.Collections.Generic;
using System.Linq;
using Config;
using Controllers.Base;
using Data;
using Data.Base;
using Data.Object;
using Extensions;
using UnityEngine;
using UnityEngine.Rendering;

namespace Systems
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
        public Vector2 followLead;
        public float followScale;
        public float followMinSize;
        public float followViewSizeThreshold;
        private float _cfgShakeScale;
        private bool _cfgEnableBloom;

        // Exposed properties
        public Rect BaseRect { get; private set; } = new(-20f, -10f, 40f, 20f);
        public Vector2 ViewPosition { get; private set; } = new(0f, 0f);
        public float ViewSize { get; private set; } = 10f;
        public Dictionary<string, Transform> FollowTargets { get; } = new();
        public Vector2 LastFollowTarget { get; private set; } = Vector2.zero;

        // Local variables
        private Vector2 _shoveOffset;
        private Vector2 _shoveVelocity;
        private float _shake;
        private float _shakeTimer;
        private bool _enableFollow;
        private bool _hasFollowTargets;
        private Rect _followRect;
        private int _smoothFollowCooldown;
        private int _lastFollowTargetCount;
        private Vector2 _lastFollowPosition;
        private float _lastCameraAspect;

        // External references
        public GameObject gCameraMount;
        public SpriteRenderer cCameraCover;
        public Volume cVolume;
        private Camera _cCamera;

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnConfigUpdate += OnConfigUpdate;
            EventSystem._.OnPlayerUnregister += OnPlayerUnregister;
            EventSystem._.OnPlayerSpawn += OnPlayerSpawn;
            EventSystem._.OnPlayerDie += OnPlayerDie;
            EventSystem._.OnMapBuild += OnMapBuild;

            _cCamera = Camera.main;

            InvokeRepeating(nameof(RareUpdate), 1f, 1f);
        }

        private void OnDestroy()
        {
            EventSystem._.OnConfigUpdate -= OnConfigUpdate;
            EventSystem._.OnPlayerUnregister -= OnPlayerUnregister;
            EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
            EventSystem._.OnPlayerDie -= OnPlayerDie;
            EventSystem._.OnMapBuild -= OnMapBuild;
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
            _hasFollowTargets = true;
        }

        public void StopFollowing(BsObject obj, bool keepMomentum = false)
        {
            StopFollowing(obj.Id, keepMomentum);
        }

        public void StopFollowing(string id, bool keepMomentum = false)
        {
            if (!FollowTargets.TryGetValue(id, out var followTarget)) return;
            if (FollowTargets.Count == 1)
            {
                _hasFollowTargets = false;
                if (keepMomentum)
                {
                    LastFollowTarget = followTarget.localPosition;
                }
            }
            FollowTargets.Remove(id);
        }

        public void ClearFollowing()
        {
            FollowTargets.Clear();
            _hasFollowTargets = false;
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
            ViewSize = viewRect.size.ForceAspect(_cCamera.aspect).y * .5f;
        }

        public void SetViewRectSmooth(Rect viewRect, float positionFactor, float sizeFactor)
        {
            ViewPosition = MathfExt.MoveToExponential(ViewPosition, viewRect.center, positionFactor);
            ViewSize = MathfExt.MoveToExponential(ViewSize,
                viewRect.size.ForceAspect(_cCamera.aspect).y * .5f, sizeFactor);
        }

        // Event functions
        private void OnConfigUpdate(ConfigDelta cfg)
        {
            _cfgShakeScale = (float)cfg.GetOrDefault("cmsys", "shake", _cfgShakeScale);
            _cfgEnableBloom = (bool)cfg.GetOrDefault("cmsys", "bloom", _cfgEnableBloom);

            cVolume.sharedProfile.components.Find(component => component.name == "Bloom").active = _cfgEnableBloom;
        }

        private void OnPlayerUnregister(BsPlayer player)
        {
            StopFollowing(player);
        }

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
            StopFollowing(player, true);
        }

        private void OnMapBuild(BsMap map)
        {
            _smoothFollowCooldown = 5;
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

            // Apply position and size
            var cameraTransform = _cCamera.transform;
            var newPosition = ViewPosition + _shoveOffset * (_cfgShakeScale * _cCamera.orthographicSize);
            cameraTransform.localPosition = new Vector3(newPosition.x, newPosition.y, cameraTransform.localPosition.z);
            _cCamera.orthographicSize = ViewSize;

            // Focus on follow targets
            if (!_enableFollow) return;
            var followPositions = _hasFollowTargets
                ? FollowTargets.Values.Select(transform => MathfExt.Clamp(transform.position, BaseRect)).ToArray()
                : new[] { MathfExt.Clamp(LastFollowTarget, BaseRect) };
            var min = followPositions[0];
            var max = followPositions[0];
            for (var i = 1; i < FollowTargets.Count; i++)
            {
                var target = followPositions[i];
                min = MathfExt.Min(target, min);
                max = MathfExt.Max(target, max);
            }
            var followPosition = MathfExt.Mean(min, max);
            var followVelocity = (followPosition - _lastFollowPosition) / Time.fixedDeltaTime;
            _lastFollowPosition = followPosition;
            var lookAhead = Vector2.zero;
            var followTargetCount = FollowTargets.Count;
            if (followVelocity.magnitude <= 1000f && followTargetCount == _lastFollowTargetCount)
            {
                lookAhead = followVelocity * (followLead.x * Mathf.Min(followVelocity.magnitude / followLead.y, 1f));
            }
            _lastFollowTargetCount = followTargetCount;
            var targetRect = new Rect(followPosition + lookAhead, Vector2.zero).Resize(MathfExt.Min(Vector2.one *
                Mathf.Max((max - min).magnitude * followScale, followMinSize), BaseRect.size));
            if (_smoothFollowCooldown == 0)
            {
                SetViewRectSmooth(targetRect, followSpeed.x * Time.fixedDeltaTime, followSpeed.y * Time.fixedDeltaTime);
            }
            else
            {
                SetViewRect(targetRect);
                _smoothFollowCooldown--;
            }
        }

        private void RareUpdate()
        {
            // Update camera size if the screen aspect ratio changes
            if (_enableFollow || Mathf.Approximately(_cCamera.aspect, _lastCameraAspect)) return;
            ViewSize = BaseRect.size.ForceAspect(_cCamera.aspect).y * .5f;
            _cCamera.orthographicSize = ViewSize;
            _lastCameraAspect = _cCamera.aspect;
        }
    }
}
