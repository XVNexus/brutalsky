using Utils.Lcs;

namespace Brutalsky.Logic
{
    public class BsLink
    {
        public (ushort, byte) FromPort { get; }
        public (ushort, byte) ToPort { get; }

        public BsLink((ushort, byte) fromPort, (ushort, byte) toPort)
        {
            FromPort = fromPort;
            ToPort = toPort;
        }

        public BsLink(ushort fromNode, byte fromPort, ushort toNode, byte toPort)
        {
            FromPort = (fromNode, fromPort);
            ToPort = (toNode, toPort);
        }

        public LcsLine ToLcs()
        {
            return new LcsLine('^', new LcsProp[]
            {
                new(LcsType.UShort, FromPort.Item1), new(LcsType.Byte, FromPort.Item2),
                new(LcsType.UShort, ToPort.Item1), new(LcsType.Byte, ToPort.Item2)
            });
        }

        public static BsLink FromLcs(LcsLine line)
        {
            return new BsLink
            (
                (ushort)line.Props[0].Value, (byte)line.Props[1].Value,
                (ushort)line.Props[2].Value, (byte)line.Props[3].Value
            );
        }
    }
}
