using System.Collections.Generic;
using UnityEngine;

namespace Controllers.Pool
{
    public class PoolWaveController : MonoBehaviour
    {
        // Constants
        public const float WavePointDensity = 2f;

        // Variables
        private float waveHeight;
        private int wavePointCount;
        private float[] wavePointOffsets;
        private List<PoolWaveEffect> waveEffects = new();

        // References
        public LineRenderer cLineRenderer;
        private PoolController cPoolController;
        private SpriteRenderer cSpriteRenderer;

        // Events
        private void OnEnable()
        {
            cPoolController = GetComponent<PoolController>();
            cSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // Set wave color to match the pool color
            cLineRenderer.material = cSpriteRenderer.material;
            cLineRenderer.sortingOrder = cSpriteRenderer.sortingOrder;

            // Set up wave renderer
            var lineWidth = cLineRenderer.widthMultiplier;
            var poolTransform = transform;
            var poolScale = poolTransform.localScale;
            cLineRenderer.positionCount = Mathf.RoundToInt(poolScale.x * WavePointDensity) + 3;
            var posCount = cLineRenderer.positionCount;
            wavePointCount = posCount - 4;
            wavePointOffsets = new float[wavePointCount];
            var surfaceOffset = lineWidth * 2f / poolScale.y;
            var edgeOffset = lineWidth * .5f / poolScale.x;
            var wavePointInterval = 1f / WavePointDensity / poolScale.x;
            waveHeight = lineWidth * .5f / poolScale.y;
            cLineRenderer.SetPosition(0, new Vector2(-.5f + edgeOffset, -surfaceOffset));
            cLineRenderer.SetPosition(1, new Vector2(-.5f + edgeOffset, 0f));
            cLineRenderer.SetPosition(posCount - 2, new Vector2(.5f - edgeOffset, 0f));
            cLineRenderer.SetPosition(posCount - 1, new Vector2(.5f - edgeOffset, -surfaceOffset));
            for (var i = 0; i < wavePointCount; i++)
            {
                var offset = -.5f + wavePointInterval * (i + 1);
                cLineRenderer.SetPosition(i + 2, new Vector2(offset, 0f));
                wavePointOffsets[i] = offset * poolScale.x;
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
            var splashPoint = transform.InverseTransformPoint(other.transform.position).x * cPoolController.bsObject.size.x;
            waveEffects.Add(new PoolWaveEffect(splashPoint, 25f, 2f, 3f));
        }

        // Updates
        private void Update()
        {
            // Update wave effects
            if (waveEffects.Count == 0) return;
            var wavePointHeights = new float[wavePointCount];
            for (var i = waveEffects.Count - 1; i >= 0; i--)
            {
                var waveEffect = waveEffects[i];
                waveEffect.Update(Time.deltaTime);
                if (waveEffect.idle)
                {
                    waveEffects.RemoveAt(i);
                    continue;
                }
                for (var j = 0; j < wavePointCount; j++)
                {
                    wavePointHeights[j] += waveEffect.SamplePoint(wavePointOffsets[j]);
                }
            }

            // Apply wave heightmap to line renderer
            for (var i = 0; i < wavePointCount; i++)
            {
                cLineRenderer.SetPosition(i + 2,
                    new Vector3(cLineRenderer.GetPosition(i + 2).x,
                        Mathf.Clamp(wavePointHeights[i], -1f, 1f) * waveHeight));
            }
        }
    }
}
