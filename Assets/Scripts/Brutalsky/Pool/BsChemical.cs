namespace Brutalsky.Pool
{
    public class BsChemical
    {
        public float buoyancy { get; private set; }
        public float viscosity { get; private set; }
        public float damage { get; private set; }

        public BsChemical(float buoyancy, float viscosity, float damage = 0f)
        {
            this.buoyancy = buoyancy;
            this.viscosity = viscosity;
            this.damage = damage;
        }

        public BsChemical()
        {
        }

        public BsChemical Modify(float? buoyancy = null, float? viscosity = null, float? damage = null)
        {
            return new BsChemical
            (
                buoyancy ?? this.buoyancy,
                viscosity ?? this.viscosity,
                damage ?? this.damage
            );
        }

        // Thin
        public static BsChemical Oil()
            => new BsChemical(2.5f, 1f);

        // Medium
        public static BsChemical Water()
            => new BsChemical(25f, 1f);

        // Thick
        public static BsChemical Honey()
            => new BsChemical(25f, 10f);

        // Healing
        public static BsChemical Medicine()
            => new BsChemical(25f, 1f, -25f);

        // Harmful
        public static BsChemical Lava()
            => new BsChemical(25f, 1f, 25f);
    }
}
