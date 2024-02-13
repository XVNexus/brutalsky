namespace Brutalsky
{
    public class BsMaterial
    {
        public float friction { get; }
        public float restitution { get; }
        public float adhesion { get; }
        public float density { get; }
        public bool dynamic { get; }

        public BsMaterial(float friction, float restitution, float adhesion, float density = 0f, bool dynamic = false)
        {
            this.friction = friction;
            this.restitution = restitution;
            this.adhesion = adhesion;
            this.density = density;
            this.dynamic = dynamic;
        }

        // Lightweight
        public static BsMaterial Wood(bool dynamic = false)
            => new BsMaterial(2f, 0f, 0f, .1f, dynamic);

        // Midweight
        public static BsMaterial Metal(bool dynamic = false)
            => new BsMaterial(2f, 0f, 0f, 1f, dynamic);

        // Heavyweight
        public static BsMaterial Stone(bool dynamic = false)
            => new BsMaterial(2f, 0f, 0f, 10f, dynamic);

        // Slippery
        public static BsMaterial Ice(bool dynamic = false)
            => new BsMaterial(0f, 0f, 0f, 1f, dynamic);

        // Bouncy
        public static BsMaterial Rubber(bool dynamic = false)
            => new BsMaterial(0f, 1f, 0f, 1f, dynamic);

        // Sticky
        public static BsMaterial Glue(bool dynamic = false)
            => new BsMaterial(0f, 0f, 5f, 1f, dynamic);
    }
}
