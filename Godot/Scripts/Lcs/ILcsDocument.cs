namespace Brutalsky.Scripts.Lcs;

public interface ILcsDocument
{
    public LcsDocument _ToLcs();

    public void _FromLcs(LcsDocument line);
}
