using UnityEngine;

namespace Brutalsky
{
    public class BsColor
    {
        public Color tint { get; set; }
        public BsLayer layer { get; set; }

        public int sortingOrder => layer switch
        {
            BsLayer.Background => -2,
            BsLayer.Midground => 0,
            BsLayer.Foreground => 2,
            _ => 0
        };

        public BsColor(Color tint, BsLayer layer = BsLayer.Midground)
        {
            this.tint = tint;
            this.layer = layer;
        }

        public static BsColor Wood(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.9f, .6f, .3f), layer);

        public static BsColor Metal(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.9f, .9f, 1f), layer);

        public static BsColor Stone(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.7f, .7f, .7f), layer);

        public static BsColor Ice(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.5f, .8f, 1f), layer);

        public static BsColor Rubber(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.7f, .4f, .9f), layer);

        public static BsColor Glue(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.7f, .9f, .4f), layer);

        public static BsColor Oil(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.3f, .3f, .4f), layer);

        public static BsColor Water(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.4f, .4f, 1f), layer);

        public static BsColor Honey(BsLayer layer = BsLayer.Midground)
            => new BsColor(new Color(.9f, .7f, 0f), layer);
    }

    public enum BsLayer
    {
        Background,
        Midground,
        Foreground
    }
}
