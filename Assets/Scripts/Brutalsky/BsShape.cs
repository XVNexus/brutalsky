namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public BsPath path { get; set; }
        public BsMaterial material { get; set; }
        public BsColor color { get; set; }
        public bool simulated { get; set; }

        public BsShape(BsTransform transform, BsPath path, BsMaterial material, BsColor color, bool simulated = true)
        {
            this.transform = transform;
            this.path = path;
            this.material = material;
            this.color = color;
            this.simulated = simulated;
        }
    }
}
