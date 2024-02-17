using Brutalsky.Property;

namespace Brutalsky.Object
{
    public class BsShape : BsObject
    {
        public BsPath path { get; set; }
        public BsMaterial material { get; set; }
        public BsColor color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

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

        public override void Parse(string[] raw)
        {
            transform = new BsTransform();
            transform.Parse(raw[0]);
            path = new BsPath();
            path.Parse(raw[1]);
            material = new BsMaterial();
            material.Parse(raw[2]);
            color = new BsColor();
            color.Parse(raw[3]);
            layer = raw[4] switch
            {
                "0" => BsLayer.Background,
                "2" => BsLayer.Foreground,
                _ => BsLayer.Midground
            };
            simulated = raw[5] == "1";
        }

        public override string[] Stringify()
        {
            return new[]
            {
                transform.Stringify(),
                path.Stringify(),
                material.Stringify(),
                color.Stringify(),
                layer switch
                {
                    BsLayer.Background => "0",
                    BsLayer.Foreground => "2",
                    _ => "1"
                },
                simulated ? "1" : "0"
            };
        }
    }
}
