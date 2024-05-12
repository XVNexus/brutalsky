namespace Brutalsky.Logic
{
    public struct BsPort
    {
        public ushort NodeId { get; }
        public byte PortId { get; }

        public BsPort(ushort nodeId, byte portId)
        {
            NodeId = nodeId;
            PortId = portId;
        }
    }
}
