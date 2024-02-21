namespace Brutalsky.Property
{
    public class BsMaterial : BsProperty
    {
        public float friction { get; private set; }
        public float restitution { get; private set; }
        public float adhesion { get; private set; }
        public float density { get; private set; }
        public float damage { get; private set; }
        public bool dynamic { get; private set; }

        public BsMaterial(float friction, float restitution, float adhesion, float density, float damage = 0f, bool dynamic = false)
        {
            this.friction = friction;
            this.restitution = restitution;
            this.adhesion = adhesion;
            this.density = density;
            this.damage = damage;
            this.dynamic = dynamic;
        }

        public BsMaterial()
        {
        }

        public BsMaterial Modify(float? friction = null, float? restitution = null, float? adhesion = null,
            float? density = null, float? damage = null)
        {
            return new BsMaterial
            (
                friction ?? this.friction,
                restitution ?? this.restitution,
                adhesion ?? this.adhesion,
                density ?? this.density,
                damage ?? this.damage,
                dynamic
            );
        }

        // Lightweight
        public static BsMaterial Wood(bool dynamic = false)
            => new BsMaterial(2f, 0f, 0f, .1f, 0f, dynamic);

        // Midweight
        public static BsMaterial Metal(bool dynamic = false)
            => new BsMaterial(2f, 0f, 0f, 1f, 0f, dynamic);

        // Heavyweight
        public static BsMaterial Stone(bool dynamic = false)
            => new BsMaterial(2f, 0f, 0f, 10f, 0f, dynamic);

        // Slippery
        public static BsMaterial Ice(bool dynamic = false)
            => new BsMaterial(0f, 0f, 0f, 1f, 0f, dynamic);

        // Bouncy
        public static BsMaterial Rubber(bool dynamic = false)
            => new BsMaterial(0f, 1f, 0f, 1f, 0f, dynamic);

        // Sticky
        public static BsMaterial Glue(bool dynamic = false)
            => new BsMaterial(5f, 0f, 1f, 1f, 0f, dynamic);

        // Healing
        public static BsMaterial Medkit(bool dynamic = false)
            => new BsMaterial(2f, 0f, 0f, 1f, -25f, dynamic);

        // Harmful
        public static BsMaterial Electric(bool dynamic = false)
            => new BsMaterial(2f, 0f, 0f, 1f, 25f, dynamic);

        public override void Parse(string raw)
        {
            var parts = raw.Split(' ');
            friction = float.Parse(parts[0]);
            restitution = float.Parse(parts[1]);
            adhesion = float.Parse(parts[2]);
            density = float.Parse(parts[3]);
            dynamic = parts[4][0] == '1';
        }

        public override string Stringify()
        {
            return $"{friction} {restitution} {adhesion} {density} {(dynamic ? '1' : '0')}";
        }
    }
}
