namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public BsPath path { get; set; }
        public BsMaterial material { get; set; }
        public BsColor color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public override char saveSymbol => 'S';

        public BsShape(BsTransform transform, BsPath path, BsMaterial material, BsColor color, BsLayer layer = BsLayer.Midground, bool simulated = true)
        {
            this.transform = transform;
            this.path = path;
            this.material = material;
            this.color = color;
            this.layer = layer;
            this.simulated = simulated;
        }

        public BsShape()
        {
        }
    }
}
