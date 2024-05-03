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

        public LcsLine ToLcs()
        {
            return new LcsLine
            (
                '^',
                new[] { LcsParser.Stringify(FromPort.Item1), LcsParser.Stringify(FromPort.Item2) },
                new[] { LcsParser.Stringify(ToPort.Item1), LcsParser.Stringify(ToPort.Item2) }
            );
        }

        public static BsLink FromLcs(LcsLine line)
        {
            return new BsLink((LcsParser.ParseInt(line.Header[0]), LcsParser.ParseInt(line.Header[1])),
                (LcsParser.ParseInt(line.Properties[0]), LcsParser.ParseInt(line.Properties[1])));
        }
    }
}
