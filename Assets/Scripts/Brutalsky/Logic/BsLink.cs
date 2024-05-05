using Utils.Lcs;

namespace Brutalsky.Logic
{
    public class BsLink
    {
        public (int, int) FromPort { get; }
        public (int, int) ToPort { get; }

        public BsLink((int, int) fromPort, (int, int) toPort)
        {
            FromPort = fromPort;
            ToPort = toPort;
        }

        public BsLink(int fromNode, int fromPort, int toNode, int toPort)
        {
            FromPort = (fromNode, fromPort);
            ToPort = (toNode, toPort);
        }

        public LcsLine ToLcs()
        {
            return new LcsLine('^', new LcsProp[]
            {
                new(LcsType.Int, FromPort.Item1), new(LcsType.Int, FromPort.Item2),
                new(LcsType.Int, ToPort.Item1), new(LcsType.Int, ToPort.Item2)
            });
        }

        public static BsLink FromLcs(LcsLine line)
        {
            return new BsLink
            (
                (int)line.Props[0].Value, (int)line.Props[1].Value,
                (int)line.Props[2].Value, (int)line.Props[3].Value
            );
        }
    }
}
