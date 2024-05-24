using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsShape : BsObject
{
    public Path Path
    {
        get => LcsInfo.Deserialize<Path>(Props[0]);
        set => Props[0] = LcsInfo.Serialize(value);
    }

    public float Friction
    {
        get => (float)Props[1];
        set => Props[1] = value;
    }

    public float Restitution
    {
        get => (float)Props[2];
        set => Props[2] = value;
    }

    public float Adhesion
    {
        get => (float)Props[3];
        set => Props[3] = value;
    }

    public float Density
    {
        get => (float)Props[4];
        set => Props[4] = value;
    }

    public float Health
    {
        get => (float)Props[5];
        set => Props[5] = value;
    }

    public bool Dynamic
    {
        get => (bool)Props[6];
        set => Props[6] = value;
    }

    public Color Color
    {
        get => new((float)((object[])Props[7])[0], (float)((object[])Props[7])[1],
            (float)((object[])Props[7])[2], (float)((object[])Props[7])[3]);
        set => Props[7] = new object[] { value.R, value.G, value.B, value.A };
    }

    public bool Glow
    {
        get => (bool)Props[8];
        set => Props[8] = value;
    }

    public (float, float, float, float, float) Material
    {
        get => (Friction, Restitution, Adhesion, Density, Health);
        set
        {
            Friction = value.Item1;
            Restitution = value.Item2;
            Adhesion = value.Item3;
            Density = value.Item4;
            Health = value.Item5;
        }
    }

    public BsShape(string id, params string[] references) : base(TypeShape, id, references)
    {
        Props = new object[] { new object[2], 0f, 0f, 0f, 0f, 0f, false, new object[4], false };
    }
}
