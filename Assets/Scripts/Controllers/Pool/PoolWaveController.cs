using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Controllers.Pool
{
    public class PoolWaveController : MonoBehaviour
    {
        // Variables
        public float wavePointDensity = 2f;
        private Vector2 buoyancyForce;
        private float surfaceAngle;
        private float surfacePosition;
        private float waveHeight;
        private int wavePointCount;
        private float[] wavePointOffsets;
        private List<WaveEffect> waveEffects = new();

        // References
        public LineRenderer cLineRenderer;
        private PoolController cPoolController;
        private SpriteRenderer cSpriteRenderer;

        // Functions
        public void SetWavePointHeight(int index, float height)
        {
            if (index < 0 || index >= wavePointCount) return;
            cLineRenderer.SetPosition(index + 2,
                new Vector3(cLineRenderer.GetPosition(index + 2).x,
                    Mathf.Clamp(height, -1f, 1f) * waveHeight));
        }

        // Events
        private void Start()
        {
            cPoolController = GetComponent<PoolController>();
            cSpriteRenderer = GetComponent<SpriteRenderer>();

            // Set wave color to match the pool color
            cLineRenderer.material = cSpriteRenderer.material;
            cLineRenderer.sortingOrder = cSpriteRenderer.sortingOrder;

            // Set up wave renderer
            var lineWidth = cLineRenderer.widthMultiplier;
            var poolTransform = transform;
            var poolScale = poolTransform.localScale;
            cLineRenderer.positionCount = Mathf.RoundToInt(poolScale.x * wavePointDensity) + 3;
            var posCount = cLineRenderer.positionCount;
            wavePointCount = posCount - 4;
            wavePointOffsets = new float[wavePointCount];
            var surfaceOffset = lineWidth * 2f / poolScale.y;
            var edgeOffset = lineWidth * .5f / poolScale.x;
            var wavePointInterval = 1f / wavePointDensity / poolScale.x;
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
            // Trigger wave splash effect
            var splashPoint = transform.InverseTransformPoint(other.transform.position).x * cPoolController.bsObject.size.x;
            waveEffects.Add(new WaveEffect(splashPoint, 25f, 2f, 3f, -1f));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Trigger wave splash effect
            var splashPoint = transform.InverseTransformPoint(other.transform.position).x * cPoolController.bsObject.size.x;
            waveEffects.Add(new WaveEffect(splashPoint, 25f, 2f, 3f));
        }

        // Updates
        private void FixedUpdate()
        {
            // Update wave effects
            if (waveEffects.Count == 0) return;
            var wavePointHeights = new float[wavePointCount];
            for (var i = waveEffects.Count - 1; i >= 0; i--)
            {
                var waveEffect = waveEffects[i];
                waveEffect.Update(Time.fixedDeltaTime);
                if (!waveEffect.active)
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
