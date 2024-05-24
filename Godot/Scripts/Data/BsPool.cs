using Godot;

namespace Brutalsky.Scripts.Data;

public class BsPool : BsObject
{
    public Vector2 Size
    {
        get => new((float)((object[])Props[0])[0], (float)((object[])Props[0])[1]);
        set => Props[0] = new object[] { value.X, value.Y };
    }

    public float Buoyancy
    {
        get => (float)Props[1];
        set => Props[1] = value;
    }

    public float Viscosity
    {
        get => (float)Props[2];
        set => Props[2] = value;
    }

    public float Healing
    {
        get => (float)Props[3];
        set => Props[3] = value;
    }

    public Color Color
    {
        get => new((float)((object[])Props[4])[0], (float)((object[])Props[4])[1],
            (float)((object[])Props[4])[2], (float)((object[])Props[4])[3]);
        set => Props[4] = new object[] { value.R, value.G, value.B, value.A };
    }

    public bool Glow
    {
        get => (bool)Props[5];
        set => Props[5] = value;
    }

    public (float, float, float) Chemical
    {
        get => (Buoyancy, Viscosity, Healing);
        set
        {
            Buoyancy = value.Item1;
            Viscosity = value.Item2;
            Healing = value.Item3;
        }
    }

    public BsPool(string id, params string[] references) : base(TypePool, id, references)
    {
        Props = new object[] { new object[2], 0f, 0f, 0f, new object[4], false };
    }
}
