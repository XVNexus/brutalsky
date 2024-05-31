using Lcs;

namespace Data
{
    public class BsLink : ILcsLine
    {
        public int From { get; set; }
        public int To { get; set; }
        public float Buffer { get; set; }

        public BsLink(int from, int to)
        {
            From = from;
            To = to;
        }

        public BsLink() { }

        public void Init()
        {
            Buffer = 0f;
        }

        public LcsLine _ToLcs()
        {
            return new LcsLine('^', From, To);
        }

        public void _FromLcs(LcsLine line)
        {
            From = line.Get<int>(0);
            To = line.Get<int>(1);
        }
    }
}
