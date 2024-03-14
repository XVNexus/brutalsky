using Utils.Ext;

namespace Brutalsky.Shape
{
    public class BsMaterial
    {
        public float Friction { get; private set; }
        public float Restitution { get; private set; }
        public float Adhesion { get; private set; }
        public float Density { get; private set; }
        public float Damage { get; private set; }
        public bool Dynamic { get; private set; }

        public BsMaterial(float friction, float restitution, float adhesion, float density, float damage = 0f, bool dynamic = false)
        {
            Friction = friction;
            Restitution = restitution;
            Adhesion = adhesion;
            Density = density;
            Damage = damage;
            Dynamic = dynamic;
        }

        public BsMaterial()
        {
        }

        public BsMaterial Modify(float? friction = null, float? restitution = null, float? adhesion = null,
            float? density = null, float? damage = null)
        {
            return new BsMaterial
            (
                friction ?? Friction,
                restitution ?? Restitution,
                adhesion ?? Adhesion,
                density ?? Density,
                damage ?? Damage,
                Dynamic
            );
        }

        // Lightweight
        public static BsMaterial Wood(bool dynamic = false) => new(2f, 0f, 0f, .1f, 0f, dynamic);

        // Midweight
        public static BsMaterial Metal(bool dynamic = false) => new(2f, 0f, 0f, 1f, 0f, dynamic);

        // Heavyweight
        public static BsMaterial Stone(bool dynamic = false) => new(2f, 0f, 0f, 10f, 0f, dynamic);

        // Slippery
        public static BsMaterial Ice(bool dynamic = false) => new(0f, 0f, 0f, 1f, 0f, dynamic);

        // Bouncy
        public static BsMaterial Rubber(bool dynamic = false) => new(0f, 1f, 0f, 1f, 0f, dynamic);

        // Sticky
        public static BsMaterial Glue(bool dynamic = false) => new(5f, 0f, 2f, 1f, 0f, dynamic);

        // Healing
        public static BsMaterial Medkit(bool dynamic = false) => new(2f, 0f, 0f, 1f, 25f, dynamic);

        // Harmful
        public static BsMaterial Electric(bool dynamic = false) => new(2f, 0f, 0f, 1f, -25f, dynamic);

        public static BsMaterial Parse(string raw)
        {
            var parts = raw.Split(' ');
            return new BsMaterial(float.Parse(parts[0]), float.Parse(parts[1]),
                float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]),
                BoolExt.Parse(parts[5]));
        }

        public override string ToString()
        {
            return $"{Friction} {Restitution} {Adhesion} {Density} {Damage} {BoolExt.ToString(Dynamic)}";
        }
    }
}
