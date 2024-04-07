namespace Utils.Shape
{
    public class ShapeMaterial
    {
        public float Friction { get; private set; }
        public float Restitution { get; private set; }
        public float Adhesion { get; private set; }
        public float Density { get; private set; }
        public float Health { get; private set; }
        public bool Dynamic { get; private set; }

        public ShapeMaterial(float friction, float restitution, float adhesion, float density, float health = 0f, bool dynamic = false)
        {
            Friction = friction;
            Restitution = restitution;
            Adhesion = adhesion;
            Density = density;
            Health = health;
            Dynamic = dynamic;
        }

        public ShapeMaterial()
        {
        }

        public ShapeMaterial Modify(float? friction = null, float? restitution = null, float? adhesion = null,
            float? density = null, float? health = null)
        {
            return new ShapeMaterial
            (
                friction ?? Friction,
                restitution ?? Restitution,
                adhesion ?? Adhesion,
                density ?? Density,
                health ?? Health,
                Dynamic
            );
        }

        // Lightweight
        public static ShapeMaterial Wood(bool dynamic = false) => new(2f, 0f, 0f, .1f, 0f, dynamic);

        // Midweight
        public static ShapeMaterial Metal(bool dynamic = false) => new(2f, 0f, 0f, 1f, 0f, dynamic);

        // Heavyweight
        public static ShapeMaterial Stone(bool dynamic = false) => new(2f, 0f, 0f, 10f, 0f, dynamic);

        // Slippery
        public static ShapeMaterial Ice(bool dynamic = false) => new(0f, 0f, 0f, 1f, 0f, dynamic);

        // Bouncy
        public static ShapeMaterial Rubber(bool dynamic = false) => new(0f, 1f, 0f, 1f, 0f, dynamic);

        // Sticky
        public static ShapeMaterial Glue(bool dynamic = false) => new(5f, 0f, 2f, 1f, 0f, dynamic);

        // Healing
        public static ShapeMaterial Medkit(bool dynamic = false) => new(2f, 0f, 0f, 1f, 25f, dynamic);

        // Harmful
        public static ShapeMaterial Electric(bool dynamic = false) => new(2f, 0f, 0f, 1f, -25f, dynamic);
    }
}
