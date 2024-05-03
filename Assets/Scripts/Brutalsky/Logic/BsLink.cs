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

        public string[] ToLcs()
        {
            return new[]
            {
                LcsParser.Stringify(FromPort.Item1), LcsParser.Stringify(FromPort.Item2),
                LcsParser.Stringify(ToPort.Item1), LcsParser.Stringify(ToPort.Item2)
            };
        }

        public static BsLink FromLcs(string[] fields)
        {
            return new BsLink((LcsParser.ParseInt(fields[0]), LcsParser.ParseInt(fields[1])),
                (LcsParser.ParseInt(fields[2]), LcsParser.ParseInt(fields[3])));
        }
    }
}
