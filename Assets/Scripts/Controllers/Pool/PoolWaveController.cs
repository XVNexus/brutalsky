using System.Collections.Generic;
using UnityEngine;

namespace Controllers.Pool
{
    public class WaveEffect
    {
        public float origin { get; private set; }
        public float spread { get; private set; }
        public float frequency { get; private set; }
        public float lifespan { get; private set; }
        public float scale { get; private set; }
        public bool active => timer < lifespan;

        private float timer;
        private float progress;
        private float width;
        private float height;

        public WaveEffect(float origin, float spread, float frequency, float lifespan, float scale = 1f)
        {
            this.origin = origin;
            this.spread = spread;
            this.frequency = frequency;
            this.lifespan = lifespan;
            this.scale = scale;
        }

        public float SamplePoint(float point)
        {
            var localPoint = Mathf.Abs(point - origin);
            return localPoint < width
                ? Mathf.Cos(localPoint * frequency - progress * spread) * height * scale
                : 0f;
        }

        public void Update(float deltaTime)
        {
            timer = Mathf.Min(timer + deltaTime, lifespan);
            progress = timer / lifespan;
            width = Mathf.PI / (frequency * 2f) + progress * spread / frequency;
            height = Mathf.Min(timer * 10f, Mathf.Pow(1f - progress, 2f));
        }
    }

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
            var color = cPoolController.bsObject.color.tint;
            cLineRenderer.startColor = color;
            cLineRenderer.endColor = color;
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
            waveEffects.Add(new WaveEffect(splashPoint, 20f, 2f, 2f, -1f));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Trigger wave splash effect
            var splashPoint = transform.InverseTransformPoint(other.transform.position).x * cPoolController.bsObject.size.x;
            waveEffects.Add(new WaveEffect(splashPoint, 20f, 2f, 2f));
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
