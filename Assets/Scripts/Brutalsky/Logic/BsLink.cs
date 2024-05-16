using Utils.Lcs;

namespace Brutalsky.Logic
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
            return new LcsLine('^', new LcsProp[] { new(LcsType.Port, FromPort), new(LcsType.Port, ToPort) });
        }

        public static BsLink FromLcs(LcsLine line)
        {
            return new BsLink((BsPort)line.Props[0].Value, (BsPort)line.Props[1].Value);
        }

        public override string ToString()
        {
            return $"LINK :: {ToLcs()}";
        }
    }
}
