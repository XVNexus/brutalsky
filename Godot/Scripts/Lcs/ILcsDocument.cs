namespace Brutalsky.Scripts.Lcs;

public interface ILcsDocument
{
    public LcsDocument ToLcs();

    public void FromLcs(LcsDocument line);
}
