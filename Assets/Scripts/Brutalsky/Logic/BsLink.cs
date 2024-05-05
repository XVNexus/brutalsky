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
            return new LcsLine('^', new[]
            {
                Stringifier.GetString(FromPort.Item1),
                Stringifier.GetString(FromPort.Item2),
                Stringifier.GetString(ToPort.Item1),
                Stringifier.GetString(ToPort.Item2),
            });
        }

        public static BsLink FromLcs(LcsLine line)
        {
            return new BsLink(Stringifier.ToInt32(line.Properties[0]), Stringifier.ToInt32(line.Properties[1]),
                Stringifier.ToInt32(line.Properties[2]), Stringifier.ToInt32(line.Properties[3]));
        }
    }
}
