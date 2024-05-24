using Brutalsky.Scripts.Lcs;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsSpawn : ILcsLine
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public int Priority { get; set; }

    public BsSpawn(Vector2 position, int priority)
    {
        Position = position;
        Priority = priority;
    }

    public BsSpawn() { }

    public LcsLine _ToLcs()
    {
        return new LcsLine('$', Position.X, Position.Y, Priority);
    }

    public void _FromLcs(LcsLine line)
    {
        var i = 0;
        Position = new Vector2((float)line.Props[i++], (float)line.Props[i++]);
        Priority = (int)line.Props[i++];
    }
}
