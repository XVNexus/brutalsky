using Godot;

namespace Brutalsky.Scripts.Data;

public class BsSensor : BsObject
{
    public Vector2 Size
    {
        get => new((float)((object[])Props[0])[0], (float)((object[])Props[0])[1]);
        set => Props[0] = new object[] { value.X, value.Y };
    }

    public bool OnEnter
    {
        get => (bool)Props[1];
        set => Props[1] = value;
    }

    public bool OnStay
    {
        get => (bool)Props[2];
        set => Props[2] = value;
    }

    public bool OnExit
    {
        get => (bool)Props[3];
        set => Props[3] = value;
    }

    public (bool, bool, bool) TriggerMode
    {
        get => (OnEnter, OnStay, OnExit);
        set
        {
            OnEnter = value.Item1;
            OnStay = value.Item2;
            OnExit = value.Item3;
        }
    }

    public BsSensor(string id, params string[] references) : base(TypeSensor, id, references)
    {
        Props = new object[] { new object[2], false, false, false };
    }
}
