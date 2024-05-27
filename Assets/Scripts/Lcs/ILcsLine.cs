namespace Lcs
{
    public interface ILcsLine
    {
        public LcsLine _ToLcs();

        public void _FromLcs(LcsLine line);
    }
}
