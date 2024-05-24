using Godot;

namespace Brutalsky.Scripts.Data;

public class BsGoal : BsObject
{
    public Vector2 Size
    {
        get => new((float)((object[])Props[0])[0], (float)((object[])Props[0])[1]);
        set => Props[0] = new object[] { value.X, value.Y };
    }

    public uint Redirect
    {
        get => (uint)Props[1];
        set => Props[1] = value;
    }

    public Color Color
    {
        get => new((float)((object[])Props[2])[0], (float)((object[])Props[2])[1],
            (float)((object[])Props[2])[2], (float)((object[])Props[2])[3]);
        set => Props[2] = new object[] { value.R, value.G, value.B, value.A };
    }

    public BsGoal(string id, params string[] references) : base(TypeGoal, id, references)
    {
        Props = new object[] { new object[2], 0u, new object[4] };
    }
}
