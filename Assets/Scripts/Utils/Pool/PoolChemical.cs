namespace Utils.Pool
{
    public class PoolChemical
    {
        public float Buoyancy { get; private set; }
        public float Viscosity { get; private set; }
        public float Health { get; private set; }

        public PoolChemical(float buoyancy, float viscosity, float health = 0f)
        {
            Buoyancy = buoyancy;
            Viscosity = viscosity;
            Health = health;
        }

        public PoolChemical()
        {
        }

        public PoolChemical Modify(float? buoyancy = null, float? viscosity = null, float? health = null)
        {
            return new PoolChemical
            (
                buoyancy ?? Buoyancy,
                viscosity ?? Viscosity,
                health ?? Health
            );
        }

        // Thin
        public static PoolChemical Oil() => new(2.5f, 1f);

        // Medium
        public static PoolChemical Water() => new(25f, 1f);

        // Thick
        public static PoolChemical Honey() => new(25f, 10f);

        // Healing
        public static PoolChemical Medicine() => new(25f, 1f, 25f);

        // Harmful
        public static PoolChemical Lava() => new(25f, 1f, -25f);

        public static PoolChemical Parse(string raw)
        {
            var parts = raw.Split(' ');
            return new PoolChemical(float.Parse(parts[0]), float.Parse(parts[1]),
                float.Parse(parts[2]));
        }

        public override string ToString()
        {
            return $"{Buoyancy} {Viscosity} {Health}";
        }
    }
}
