namespace Data.Logic
{
    public struct BsPort
    {
        public string NodeId { get; }
        public int PortId { get; }

        public BsPort(string nodeId, int portId)
        {
            NodeId = nodeId;
            PortId = portId;
        }
    }
}
