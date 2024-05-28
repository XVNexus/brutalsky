using Lcs;

namespace Data.Logic
{
    public struct BsLink
    {
        public BsPort FromPort { get; }
        public BsPort ToPort { get; }

        public BsLink(BsPort fromPort, BsPort toPort)
        {
            FromPort = fromPort;
            ToPort = toPort;
        }

        public BsLink(string fromNode, int fromPort, string toNode, int toPort)
        {
            FromPort = new BsPort(fromNode, fromPort);
            ToPort = new BsPort(toNode, toPort);
        }
    }
}
