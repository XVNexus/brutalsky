namespace Brutalsky.Property
{
    public class BsChemical : BsProperty
    {
        public float buoyancy { get; private set; }
        public float viscosity { get; private set; }

        public BsChemical(float buoyancy, float viscosity)
        {
            this.buoyancy = buoyancy;
            this.viscosity = viscosity;
        }

        public BsChemical()
        {
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

        public override void Parse(string raw)
        {
            var parts = raw.Split(' ');
            buoyancy = float.Parse(parts[0]);
            viscosity = float.Parse(parts[1]);
        }

        public override string Stringify()
        {
            return $"{buoyancy} {viscosity}";
        }
    }
}
