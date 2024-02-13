namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public BsPath path { get; set; }
        public BsMaterial material { get; set; }
        public BsColor color { get; set; }

        public BsShape(BsTransform transform, BsPath path, BsMaterial material, BsColor color)
        {
            this.transform = transform;
            this.path = path;
            this.material = material;
            this.color = color;
        }
    }
}
