using Lcs;

namespace Data
{
    public class BsLink : ILcsLine
    {
        public int FromNode { get; set; }
        public string FromPort { get; set; }
        public int ToNode { get; set; }
        public string ToPort { get; set; }

        public BsLink(int fromNode, string fromPort, int toNode, string toPort)
        {
            FromNode = fromNode;
            FromPort = fromPort;
            ToNode = toNode;
            ToPort = toPort;
        }

        public BsLink() { }

        public LcsLine _ToLcs()
        {
            return new LcsLine('%', FromNode, FromPort, ToNode, ToPort);
        }

        public void _FromLcs(LcsLine line)
        {
            FromNode = line.Get<int>(0);
            FromPort = line.Get<string>(1);
            ToNode = line.Get<int>(2);
            ToPort = line.Get<string>(3);
        }
    }
}
