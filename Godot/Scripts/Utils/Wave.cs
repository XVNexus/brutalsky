using Godot;

namespace Brutalsky.Scripts.Utils;

public struct Wave
{
    public float Origin { get; } = 0f;
    public float Spread { get; } = 0f;
    public float Frequency { get; } = 0f;
    public float Lifespan { get; } = 0f;
    public bool Idle { get; private set; } = false;

    private float _timer = 0f;
    private float _progress = 0f;
    private float _width = 0f;
    private float _height = 0f;

    public Wave(float origin, float spread, float frequency, float lifespan)
    {
        Origin = origin;
        Spread = spread;
        Frequency = frequency;
        Lifespan = lifespan;
    }

    public float SamplePoint(float point)
    {
        var localPoint = Mathf.Abs(point - Origin);
        return localPoint < _width
            ? -Mathf.Cos(localPoint * Frequency - _progress * Spread) * (1f - localPoint / _width) * _height * 1.5f
            : 0f;
    }

    public void Update(float deltaTime)
    {
        _timer = Mathf.Min(_timer + deltaTime, Lifespan);
        _progress = _timer / Lifespan;
        _width = Mathf.Pi / (Frequency * 2f) + _progress * Spread / Frequency;
        _height = Mathf.Min(_timer * 10f, Mathf.Pow(1f - _progress, 3f));
        Idle = _timer >= Lifespan;
    }
}