namespace Lcs
{
    public interface ILcsDocument
    {
        public LcsDocument _ToLcs();

        public void _FromLcs(LcsDocument document);
    }
}
