using UnityEngine;

namespace Controllers.Pool
{
    public class PoolWaveEffect
    {
        public float origin { get; private set; }
        public float spread { get; private set; }
        public float frequency { get; private set; }
        public float lifespan { get; private set; }
        public bool idle { get; private set; }

        private float timer;
        private float progress;
        private float width;
        private float height;

        public PoolWaveEffect(float origin, float spread, float frequency, float lifespan)
        {
            this.origin = origin;
            this.spread = spread;
            this.frequency = frequency;
            this.lifespan = lifespan;
        }

        public float SamplePoint(float point)
        {
            var localPoint = Mathf.Abs(point - origin);
            return localPoint < width
                ? -Mathf.Cos(localPoint * frequency - progress * spread) * (1f - localPoint / width) * height * 1.5f
                : 0f;
        }

        public void Update(float deltaTime)
        {
            timer = Mathf.Min(timer + deltaTime, lifespan);
            progress = timer / lifespan;
            width = Mathf.PI / (frequency * 2f) + progress * spread / frequency;
            height = Mathf.Min(timer * 10f, Mathf.Pow(1f - progress, 3f));
            idle = Mathf.Approximately(timer, lifespan);
        }
    }
}
