using System.Collections.Generic;
using Brutalsky.Object;
using Controllers.Base;
using UnityEngine;
using Utils.Ext;
using Utils.Pool;

namespace Controllers.Pool
{
    public class PoolWaveController : SubControllerBase<BsPool>
    {
        // Controller metadata
        public override string Id => "wave";
        public override bool IsUnused => !Master.Object.Simulated;

        // Config options
        public float wavePointDensity;

        // Local variables
        private float _waveHeight;
        private int _wavePointCount;
        private float[] _wavePointOffsets;
        private readonly List<PoolWave> _waves = new();

        // External references
        public LineRenderer cLineRenderer;
        private SpriteRenderer _cSpriteRenderer;

        // Init functions
        protected override void OnInit()
        {
            _cSpriteRenderer = GetComponent<SpriteRenderer>();

            // Adjust pool size to make room for waves
            var lineWidth = cLineRenderer.widthMultiplier;
            var poolTransform = transform;
            var poolScale = poolTransform.localScale;
            poolScale.y -= lineWidth * .5f;
            poolTransform.localScale = poolScale;
            var surfaceAngle = poolTransform.rotation.eulerAngles.z + 90f;
            var poolPosition = poolTransform.localPosition;
            poolPosition -= (Vector3)MathfExt.RotateVector(new Vector2(lineWidth * .25f, 0f), surfaceAngle);
            poolTransform.localPosition = poolPosition;

            // Set up wave renderer
            cLineRenderer.material = _cSpriteRenderer.material;
            cLineRenderer.sortingOrder = _cSpriteRenderer.sortingOrder;
            cLineRenderer.positionCount = Mathf.RoundToInt(poolScale.x * wavePointDensity) + 3;
            var posCount = cLineRenderer.positionCount;
            _wavePointCount = posCount - 4;
            _wavePointOffsets = new float[_wavePointCount];
            var surfaceOffset = lineWidth * 2f / poolScale.y;
            var edgeOffset = lineWidth * .5f / poolScale.x;
            var wavePointInterval = 1f / wavePointDensity / poolScale.x;
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

        // Event functions
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
            var splashPoint = transform.InverseTransformPoint(other.transform.localPosition).x * Master.Object.Size.x;
            _waves.Add(new PoolWave(splashPoint, 50f, 2.5f, 5f));
        }

        private void Update()
        {
            // Generate idle wave animation
            var wavePointHeights = new float[_wavePointCount];
            for (var i = 0; i < _wavePointCount; i++)
            {
                wavePointHeights[i] = Mathf.Sin((_wavePointOffsets[i] + Time.timeSinceLevelLoad) * 2f) * .25f;
            }

            // Update wave effects
            for (var i = _waves.Count - 1; i >= 0; i--)
            {
                var wave = _waves[i];
                wave.Update(Time.deltaTime);
                for (var j = 0; j < _wavePointCount; j++)
                {
                    wavePointHeights[j] += wave.SamplePoint(_wavePointOffsets[j]);
                }
                if (!wave.Idle) continue;
                _waves.RemoveAt(i);
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
