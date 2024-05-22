using Brutalsky.Scripts.Lcs;

namespace Brutalsky.Scripts.Map;

public struct BsLink : ILcsLine
{
    public BsPort FromPort { get; private set; }
    public BsPort ToPort { get; private set; }

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

    public void FromLcs(LcsLine line)
    {
        FromPort = (BsPort)line.Props[0];
        ToPort = (BsPort)line.Props[1];
    }

    public override string ToString()
    {
        return $"LINK :: {FromPort} > {ToPort}";
    }
}
