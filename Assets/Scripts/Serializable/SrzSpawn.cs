namespace Serializable
{
    public class SrzSpawn
    {
        public string ps { get; set; }
        public int pr { get; set; }

        public SrzSpawn(string position, int priority)
        {
            ps = position;
            pr = priority;
        }

        public SrzSpawn()
        {
        }
    }
}
