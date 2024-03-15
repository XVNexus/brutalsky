namespace Brutalsky.Pool
{
    public class BsChemical
    {
        public float Buoyancy { get; private set; }
        public float Viscosity { get; private set; }
        public float Health { get; private set; }

        public BsChemical(float buoyancy, float viscosity, float health = 0f)
        {
            Buoyancy = buoyancy;
            Viscosity = viscosity;
            Health = health;
        }

        public BsChemical()
        {
        }

        public BsChemical Modify(float? buoyancy = null, float? viscosity = null, float? health = null)
        {
            return new BsChemical
            (
                buoyancy ?? Buoyancy,
                viscosity ?? Viscosity,
                health ?? Health
            );
        }

        // Thin
        public static BsChemical Oil() => new(2.5f, 1f);

        // Medium
        public static BsChemical Water() => new(25f, 1f);

        // Thick
        public static BsChemical Honey() => new(25f, 10f);

        // Healing
        public static BsChemical Medicine() => new(25f, 1f, 25f);

        // Harmful
        public static BsChemical Lava() => new(25f, 1f, -25f);

        public static BsChemical Parse(string raw)
        {
            var parts = raw.Split(' ');
            return new BsChemical(float.Parse(parts[0]), float.Parse(parts[1]),
                float.Parse(parts[2]));
        }

        public override string ToString()
        {
            return $"{Buoyancy} {Viscosity} {Health}";
        }
    }
}
