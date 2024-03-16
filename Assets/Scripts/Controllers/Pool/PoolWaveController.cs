using System.Collections.Generic;
using Brutalsky;
using UnityEngine;
using Utils.Ext;
using Utils.Pool;

namespace Controllers.Pool
{
    public class PoolWaveController : SubControllerBase<BsPool>
    {
        public override string Id => "wave";
        public override bool IsUnused => !Master.Object.Simulated;

        public const float WavePointDensity = 2f;

        public LineRenderer cLineRenderer;
        private SpriteRenderer _cSpriteRenderer;

        private float _waveHeight;
        private int _wavePointCount;
        private float[] _wavePointOffsets;
        private readonly List<PoolWave> _waves = new();

        protected override void OnInit()
        {
            _cSpriteRenderer = GetComponent<SpriteRenderer>();

            // Adjust pool size to make room for waves
            var lineWidth = cLineRenderer.widthMultiplier;
            var poolTransform = transform;
            var poolScale = poolTransform.localScale;
            poolScale.y -= lineWidth * .5f;
            poolTransform.localScale = poolScale;
            var surfaceAngle = (poolTransform.rotation.eulerAngles.z + 90f) * Mathf.Deg2Rad;
            var poolPosition = poolTransform.position;
            poolPosition -= (Vector3)MathfExt.RotateVector(new Vector2(lineWidth * .25f, 0f), surfaceAngle);
            poolTransform.position = poolPosition;

            // Set up wave renderer
            cLineRenderer.material = _cSpriteRenderer.material;
            cLineRenderer.sortingOrder = _cSpriteRenderer.sortingOrder;
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
            var splashPoint = transform.InverseTransformPoint(other.transform.position).x * Master.Object.Size.x;
            _waves.Add(new PoolWave(splashPoint, 25f, 2f, 3f));
        }

        private void Update()
        {
            // Update wave effects
            if (_waves.Count == 0) return;
            var wavePointHeights = new float[_wavePointCount];
            for (var i = _waves.Count - 1; i >= 0; i--)
            {
                var wave = _waves[i];
                wave.Update(Time.deltaTime);
                if (wave.Idle)
                {
                    _waves.RemoveAt(i);
                    continue;
                }
                for (var j = 0; j < _wavePointCount; j++)
                {
                    wavePointHeights[j] += wave.SamplePoint(_wavePointOffsets[j]);
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
