namespace Brutalsky
{
    public class BsChemical
    {
        public float buoyancy { get; }
        public float viscosity { get; }

        public BsChemical(float buoyancy, float viscosity)
        {
            this.buoyancy = buoyancy;
            this.viscosity = viscosity;
        }

        // Low buoyancy
        public static BsChemical Oil()
            => new BsChemical(1f, 1f);

        // Medium viscosity
        public static BsChemical Water()
            => new BsChemical(10f, 1f);

        // High viscosity
        public static BsChemical Honey()
            => new BsChemical(10f, 10f);
    }
}
