namespace Brutalsky.Scripts.Data;

public class BsWeld : BsObject
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

    public BsWeld(string id, params string[] references) : base(TypeWeld, id, references)
    {
        Props = new object[] { "", "" };
    }
}
