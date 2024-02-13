using UnityEngine;

namespace Brutalsky
{
    public class BsColor
    {
        public BsLayer layer { get; set; }
        public Color tint { get; set; }
        public bool glow { get; set; }

        public int sortingOrder => layer switch
        {
            BsLayer.Background => -2,
            BsLayer.Midground => 0,
            BsLayer.Foreground => 2,
            _ => 0
        };

        public BsColor(BsLayer layer, Color tint, bool glow = false)
        {
            this.layer = layer;
            this.tint = tint;
            this.glow = glow;
        }

        public static BsColor Wood(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.9f, .6f, .35f), glow);

        public static BsColor Metal(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.9f, .9f, 1f), glow);

        public static BsColor Stone(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.7f, .7f, .7f), glow);

        public static BsColor Ice(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.5f, .8f, 1f), glow);

        public static BsColor Rubber(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.7f, .9f, .4f), glow);

        public static BsColor Glue(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.0f, .7f, 0f), glow);

        public static BsColor Oil(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.3f, .3f, .4f), glow);

        public static BsColor Water(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.4f, .4f, 1f), glow);

        public static BsColor Honey(BsLayer layer, bool glow = false)
            => new BsColor(layer, new Color(.9f, .7f, 0f), glow);
    }

    public enum BsLayer
    {
        Background,
        Midground,
        Foreground
    }
}
