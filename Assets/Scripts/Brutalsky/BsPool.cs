using UnityEngine;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public Vector2 size { get; set; }
        public BsChemical chemical { get; set; }
        public BsColor color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public override char saveSymbol => 'P';

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

        public override void Parse(string[][] raw)
        {
            transform = new BsTransform(float.Parse(raw[0][0]), float.Parse(raw[0][1]), float.Parse(raw[0][2]));
            size = new Vector2(float.Parse(raw[1][0]), float.Parse(raw[1][1]));
            chemical = new BsChemical(float.Parse(raw[2][0]), float.Parse(raw[2][1]));
            color = new BsColor(float.Parse(raw[3][0]), float.Parse(raw[3][1]), float.Parse(raw[3][2]), float.Parse(raw[3][3]), raw[3][4] == "1");
            layer = raw[4][0] switch
            {
                "0" => BsLayer.Background,
                "2" => BsLayer.Foreground,
                _ => BsLayer.Midground
            };
            simulated = raw[5][0] == "1";
        }

        public override string[][] Stringify()
        {
            return new[]
            {
                new[] { transform.position.x.ToString(), transform.position.y.ToString(), transform.rotation.ToString() },
                new[] { size.x.ToString(), size.y.ToString() },
                new[] { chemical.buoyancy.ToString(), chemical.viscosity.ToString() },
                new[] { color.tint.r.ToString(), color.tint.g.ToString(), color.tint.b.ToString(), color.tint.a.ToString(), color.glow ? "1" : "0" },
                new[]
                {
                    layer switch
                    {
                        BsLayer.Background => "0",
                        BsLayer.Foreground => "2",
                        _ => "1"
                    }
                },
                new[] { simulated ? "1" : "0" }
            };
        }
    }
}
