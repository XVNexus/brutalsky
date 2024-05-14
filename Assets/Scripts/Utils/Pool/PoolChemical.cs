namespace Utils.Pool
{
    public struct PoolChemical
    {
        public static readonly PoolChemical Oil = new(2.5f, 1f);
        public static readonly PoolChemical Water = new(25f, 1f);
        public static readonly PoolChemical Honey = new(25f, 10f);
        public static readonly PoolChemical Medicine = new(25f, 1f, 50f);
        public static readonly PoolChemical Lava = new(25f, 1f, -50f);

        public float Buoyancy { get; private set; }
        public float Viscosity { get; private set; }
        public float Health { get; private set; }

        public PoolChemical(float buoyancy, float viscosity, float health = 0f)
        {
            Buoyancy = buoyancy;
            Viscosity = viscosity;
            Health = health;
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

        public override string ToString()
        {
            return $"[B:{Buoyancy}, V:{Viscosity}, H:{Health}]";
        }
    }
}
