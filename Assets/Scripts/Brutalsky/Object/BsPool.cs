using Brutalsky.Property;
using UnityEngine;

namespace Brutalsky.Object
{
    public class BsPool : BsObject
    {
        public Vector2 size { get; set; }
        public BsChemical chemical { get; set; }
        public BsColor color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public BsPool(BsTransform transform, Vector2 size, BsChemical chemical, BsColor color, BsLayer layer = BsLayer.Midground, bool simulated = true)
        {
            this.transform = transform;
            this.size = size;
            this.chemical = chemical;
            this.color = color;
            this.layer = layer;
            this.simulated = simulated;
        }

        public BsPool()
        {
        }

        public override void Parse(string[] raw)
        {
            transform = new BsTransform();
            transform.Parse(raw[0]);
            var sizeParts = raw[1].Split(' ');
            size = new Vector2(float.Parse(sizeParts[1]), float.Parse(sizeParts[2]));
            chemical = new BsChemical();
            chemical.Parse(raw[2]);
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
                $"{size.x} {size.y}",
                chemical.Stringify(),
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
