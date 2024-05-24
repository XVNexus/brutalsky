using Godot;

namespace Brutalsky.Scripts.Data;

public class BsMount : BsObject
{
    public Vector2 EjectionForce
    {
        get => new((float)((object[])Props[0])[0], (float)((object[])Props[0])[1]);
        set => Props[0] = new object[] { value.X, value.Y };
    }

    public BsMount(string id, params string[] references) : base(TypeMount, id, references)
    {
        Props = new object[] { new object[2] };
    }
}
