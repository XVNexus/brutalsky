using Godot;

namespace Brutalsky.Scripts.Map;

public struct BsSpawn
{
    public Vector2 Position { get; }
    public int Priority { get; }

    public int Usages { get; private set; } = 0;

    public BsSpawn(Vector2 position, int priority)
    {
        Position = position;
        Priority = priority;
    }

    public Vector2 Use()
    {
        Usages++;
        return Position;
    }

    public void Reset()
    {
        Usages = 0;
    }
}
