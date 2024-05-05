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
                Stringifier.Str<int>(FromPort.Item1),
                Stringifier.Str<int>(FromPort.Item2),
                Stringifier.Str<int>(ToPort.Item1),
                Stringifier.Str<int>(ToPort.Item2),
            });
        }

        public static BsLink FromLcs(LcsLine line)
        {
            return new BsLink(Stringifier.Par<int>(line.Properties[0]), Stringifier.Par<int>(line.Properties[1]),
                Stringifier.Par<int>(line.Properties[2]), Stringifier.Par<int>(line.Properties[3]));
        }
    }
}
