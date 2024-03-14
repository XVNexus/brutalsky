using System.Collections.Generic;
using UnityEngine;
using Utils.Pool;

namespace Controllers.Pool
{
    public class PoolWaveController : MonoBehaviour
    {
        // Constants
        public const float WavePointDensity = 2f;

        // Variables
        private float _waveHeight;
        private int _wavePointCount;
        private float[] _wavePointOffsets;
        private readonly List<PoolWaveEffect> _waveEffects = new();

        // References
        public LineRenderer cLineRenderer;
        private PoolController _cPoolController;
        private SpriteRenderer _cSpriteRenderer;

        // Events
        private void Start()
        {
            _cPoolController = GetComponent<PoolController>();
            _cSpriteRenderer = GetComponent<SpriteRenderer>();

            // Set wave color to match the pool color
            cLineRenderer.material = _cSpriteRenderer.material;
            cLineRenderer.sortingOrder = _cSpriteRenderer.sortingOrder;

            // Set up wave renderer
            var lineWidth = cLineRenderer.widthMultiplier;
            var poolTransform = transform;
            var poolScale = poolTransform.localScale;
            cLineRenderer.positionCount = Mathf.RoundToInt(poolScale.x * WavePointDensity) + 3;
            var posCount = cLineRenderer.positionCount;
            _wavePointCount = posCount - 4;
            _wavePointOffsets = new float[_wavePointCount];
            var surfaceOffset = lineWidth * 2f / poolScale.y;
            var edgeOffset = lineWidth * .5f / poolScale.x;
            var wavePointInterval = 1f / WavePointDensity / poolScale.x;
            _waveHeight = lineWidth * .5f / poolScale.y;
            cLineRenderer.SetPosition(0, new Vector2(-.5f + edgeOffset, -surfaceOffset));
            cLineRenderer.SetPosition(1, new Vector2(-.5f + edgeOffset, 0f));
            cLineRenderer.SetPosition(posCount - 2, new Vector2(.5f - edgeOffset, 0f));
            cLineRenderer.SetPosition(posCount - 1, new Vector2(.5f - edgeOffset, -surfaceOffset));
            for (var i = 0; i < _wavePointCount; i++)
            {
                var offset = -.5f + wavePointInterval * (i + 1);
                cLineRenderer.SetPosition(i + 2, new Vector2(offset, 0f));
                _wavePointOffsets[i] = offset * poolScale.x;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTrigger2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnTrigger2D(other);
        }

        private void OnTrigger2D(Component other)
        {
            // Trigger wave splash effect
            var splashPoint = transform.InverseTransformPoint(other.transform.position).x * _cPoolController.BsObject.Size.x;
            _waveEffects.Add(new PoolWaveEffect(splashPoint, 25f, 2f, 3f));
        }

        // Updates
        private void Update()
        {
            // Update wave effects
            if (_waveEffects.Count == 0) return;
            var wavePointHeights = new float[_wavePointCount];
            for (var i = _waveEffects.Count - 1; i >= 0; i--)
            {
                var waveEffect = _waveEffects[i];
                waveEffect.Update(Time.deltaTime);
                if (waveEffect.Idle)
                {
                    _waveEffects.RemoveAt(i);
                    continue;
                }
                for (var j = 0; j < _wavePointCount; j++)
                {
                    wavePointHeights[j] += waveEffect.SamplePoint(_wavePointOffsets[j]);
                }
            }

            // Apply wave heightmap to line renderer
            for (var i = 0; i < _wavePointCount; i++)
            {
                cLineRenderer.SetPosition(i + 2,
                    new Vector3(cLineRenderer.GetPosition(i + 2).x,
                        Mathf.Clamp(wavePointHeights[i], -1f, 1f) * _waveHeight));
            }
        }
    }
}
