namespace Brutalsky.Scripts.Data;

public class BsSlider : BsJoint
{
    public float Length
    {
        get => (float)Props[4];
        set => Props[4] = value;
    }

    public float InitialOffset
    {
        get => (float)Props[5];
        set => Props[5] = value;
    }

    public BsSlider(string id, params string[] references) : base(TypeSlider, id, references)
    {
        Props = new object[] { "", "", 0f, false, 50f, 25f };
    }
}
