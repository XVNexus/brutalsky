namespace Utils.Shape
{
    public class ShapeMaterial
    {
        public static readonly ShapeMaterial Wood = new(2f, 0f, 0f, .1f);
        public static readonly ShapeMaterial Metal = new(2f, 0f, 0f, 1f);
        public static readonly ShapeMaterial Stone = new(2f, 0f, 0f, 10f);
        public static readonly ShapeMaterial Ice = new(0f, 0f, 0f, 1f);
        public static readonly ShapeMaterial Rubber = new(0f, 1f, 0f, 1f);
        public static readonly ShapeMaterial Glue = new(5f, 0f, 2f, 1f);
        public static readonly ShapeMaterial Medkit = new(2f, 0f, 0f, 1f, 50f);
        public static readonly ShapeMaterial Electric = new(2f, 0f, 0f, 1f, -50f);

        public float Friction { get; private set; }
        public float Restitution { get; private set; }
        public float Adhesion { get; private set; }
        public float Density { get; private set; }
        public float Health { get; private set; }

        public ShapeMaterial(float friction, float restitution, float adhesion, float density, float health = 0f)
        {
            Friction = friction;
            Restitution = restitution;
            Adhesion = adhesion;
            Density = density;
            Health = health;
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
                health ?? Health
            );
        }
    }
}
