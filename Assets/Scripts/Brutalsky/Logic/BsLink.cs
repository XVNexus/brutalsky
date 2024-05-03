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

        public string ToLcs(int hexWidth)
        {
            return LcsParser.IntToHex(FromPort.Item1, hexWidth) + LcsParser.IntToHex(FromPort.Item2, hexWidth)
                 + LcsParser.IntToHex(ToPort.Item1, hexWidth) + LcsParser.IntToHex(ToPort.Item2, hexWidth);
        }

        public static BsLink FromLcs(string property, int hexWidth)
        {
            return new BsLink((LcsParser.HexToInt(property[..hexWidth]), LcsParser.HexToInt(property[hexWidth..(hexWidth * 2)])),
                (LcsParser.HexToInt(property[(hexWidth * 2)..(hexWidth * 3)]), LcsParser.HexToInt(property[(hexWidth * 3)..(hexWidth * 4)])));
        }
    }
}
