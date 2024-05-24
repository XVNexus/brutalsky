namespace Brutalsky.Scripts.Data;

public class BsSpring : BsJoint
{
    public float Length
    {
        get => (float)Props[4];
        set => Props[4] = value;
    }

    public float RestLength
    {
        get => (float)Props[5];
        set => Props[5] = value;
    }

    public float Stiffness
    {
        get => (float)Props[6];
        set => Props[6] = value;
    }

    public float Damping
    {
        get => (float)Props[7];
        set => Props[7] = value;
    }

    public BsSpring(string id, params string[] references) : base(TypeSpring, id, references)
    {
        Props = new object[] { "", "", 0f, false, 50f, 0f, 20f, 1f };
    }
}
