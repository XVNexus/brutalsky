namespace Brutalsky.Scripts.Lcs;

public interface ILcsLine
{
    public LcsLine ToLcs();

    public void FromLcs(LcsLine line);
}
