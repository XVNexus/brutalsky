namespace Brutalsky.Scripts.Data;

public class BsJoint : BsObject
{
    public string Shape1
    {
        get => (string)Props[0];
        set => Props[0] = value;
    }

    public string Shape2
    {
        get => (string)Props[1];
        set => Props[1] = value;
    }

    public float Bias
    {
        get => (float)Props[2];
        set => Props[2] = value;
    }

    public bool Collision
    {
        get => (bool)Props[3];
        set => Props[3] = value;
    }

    protected BsJoint(int type, string id, params string[] references) : base(type, id, references) { }
}
