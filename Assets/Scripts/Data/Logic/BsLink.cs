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

        public BsLink(ushort fromNode, byte fromPort, ushort toNode, byte toPort)
        {
            FromPort = new BsPort(fromNode, fromPort);
            ToPort = new BsPort(toNode, toPort);
        }

        public LcsLine ToLcs()
        {
            return new LcsLine('^', FromPort, ToPort);
        }

        public static BsLink FromLcs(LcsLine line)
        {
            return new BsLink((BsPort)line.Props[0], (BsPort)line.Props[1]);
        }

        public override string ToString()
        {
            return $"LINK :: {FromPort} > {ToPort}";
        }
    }
}
