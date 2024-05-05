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
    }
}
