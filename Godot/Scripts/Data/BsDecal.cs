using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsDecal : BsObject
{
    public Path Path
    {
        get => LcsInfo.Deserialize<Path>(Props[0]);
        set => Props[0] = LcsInfo.Serialize(value);
    }

    public Color Color
    {
        get => new((float)((object[])Props[1])[0], (float)((object[])Props[1])[1],
            (float)((object[])Props[1])[2], (float)((object[])Props[1])[3]);
        set => Props[1] = new object[] { value.R, value.G, value.B, value.A };
    }

    public bool Glow
    {
        get => (bool)Props[2];
        set => Props[2] = value;
    }

    public BsDecal(string id, params string[] references) : base(TypeDecal, id, references)
    {
        Props = new object[] { new object[2], new object[4], false };
    }
}
