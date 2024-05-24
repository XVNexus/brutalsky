using Brutalsky.Scripts.Lcs;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsPlayer : ILcsLine
{
    public const int TypeDummy = 0;
    public const int TypeMain = 1;
    public const int TypeBot = 2;
    public const int TypeLocal1 = 3;
    public const int TypeLocal2 = 4;

    public int Type { get; set; } = TypeDummy;
    public Color Color { get; set; } = new(1f, 1f, 1f);
    public float Health { get; set; } = 100f;

    public BsPlayer() { }

    public LcsLine _ToLcs()
    {
        return new LcsLine('@', Type, Color.R, Color.G, Color.B, Color.A, Health);
    }

    public void _FromLcs(LcsLine line)
    {
        var i = 0;
        Type = (int)line.Props[i++];
        Color = new Color
        (
            (float)line.Props[i++],
            (float)line.Props[i++],
            (float)line.Props[i++],
            (float)line.Props[i++]
        );
        Health = (float)line.Props[i++];
    }
}
