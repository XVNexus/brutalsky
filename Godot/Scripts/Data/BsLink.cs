using Brutalsky.Scripts.Lcs;

namespace Brutalsky.Scripts.Data;

public class BsLink : ILcsLine
{
    public string FromPort { get; set; } = "";
    public string ToPort { get; set; } = "";

    public BsLink(string fromPort, string toPort)
    {
        FromPort = fromPort;
        ToPort = toPort;
    }

    public BsLink(string fromNode, int fromPort, string toNode, int toPort)
    {
        FromPort = $"{fromNode} {fromPort}";
        ToPort = $"{toNode} {toPort}";
    }

    public BsLink() { }

    public LcsLine _ToLcs()
    {
        return new LcsLine('^', FromPort, ToPort);
    }

    public void _FromLcs(LcsLine line)
    {
        FromPort = (string)line.Props[0];
        ToPort = (string)line.Props[1];
    }
}
