using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsPlayer : ILcsLine
{
    public const int TypeDummy = 0;
    public const int TypeLocal1 = 1;
    public const int TypeLocal2 = 2;
    public const int TypeLocal3 = 3;
    public const int TypeLocal4 = 4;
    public const int TypeMain = 5;
    public const int TypeBot = 6;

    public uint Id { get; set; }
    public string Name { get; set; } = "Anonymous";
    public int Type { get; set; } = TypeDummy;
    public Color Color { get; set; } = new(1f, 1f, 1f);

    public BsPlayer(string name, int type)
    {
        Id = Tags.GenerateId(name);
        Name = name;
        Type = type;
    }

    public BsPlayer() { }

    public LcsLine _ToLcs()
    {
        return new LcsLine('@', Id, Name, Type, Color.R, Color.G, Color.B, Color.A);
    }

    public void _FromLcs(LcsLine line)
    {
        var i = 0;
        Id = (uint)line.Props[i++];
        Name = (string)line.Props[i++];
        Type = (int)line.Props[i++];
        Color = new Color
        (
            (float)line.Props[i++],
            (float)line.Props[i++],
            (float)line.Props[i++],
            (float)line.Props[i++]
        );
    }
}
