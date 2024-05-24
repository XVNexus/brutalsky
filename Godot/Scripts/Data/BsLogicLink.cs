using Brutalsky.Scripts.Lcs;

namespace Brutalsky.Scripts.Data;

public class BsLogicLink : ILcsLine
{
    public long FromPort { get; set; }
    public long ToPort { get; set; }

    public BsLogicLink(long fromPort, long toPort)
    {
        FromPort = fromPort;
        ToPort = toPort;
    }

    public BsLogicLink(int fromNode, int fromPort, int toNode, int toPort)
    {
        FromPort = ComposePort(fromNode, fromPort);
        ToPort = ComposePort(toNode, toPort);
    }

    public BsLogicLink() { }

    public LcsLine _ToLcs()
    {
        return new LcsLine('^', FromPort, ToPort);
    }

    public void _FromLcs(LcsLine line)
    {
        FromPort = (long)line.Props[0];
        ToPort = (long)line.Props[1];
    }

    public static long ComposePort(int nodeId, int portId)
    {
        return ((long)nodeId << 32) & portId;
    }

    public static (int, int) DecomposePort(long port)
    {
        return ((int)(port >> 32), (int)port);
    }
}
