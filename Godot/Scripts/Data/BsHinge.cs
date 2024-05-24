namespace Brutalsky.Scripts.Data;

public class BsHinge : BsJoint
{
    public float Softness
    {
        get => (float)Props[4];
        set => Props[4] = value;
    }

    public bool LimitEnabled
    {
        get => (bool)Props[5];
        set => Props[5] = value;
    }

    public float LimitLower
    {
        get => (float)Props[6];
        set => Props[6] = value;
    }

    public float LimitUpper
    {
        get => (float)Props[7];
        set => Props[7] = value;
    }

    public bool MotorEnabled
    {
        get => (bool)Props[8];
        set => Props[8] = value;
    }

    public float MotorSpeed
    {
        get => (float)Props[9];
        set => Props[9] = value;
    }

    public BsHinge(string id, params string[] references) : base(TypeHinge, id, references)
    {
        Props = new object[] { "", "", 0f, false, 0f, false, 0f, 0f, false, 0f };
    }
}
