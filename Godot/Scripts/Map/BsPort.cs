namespace Brutalsky.Scripts.Map;

public struct BsPort
{
    public int NodeId { get; }
    public int PortId { get; }

    public BsPort(int nodeId, int portId)
    {
        NodeId = nodeId;
        PortId = portId;
    }
}
