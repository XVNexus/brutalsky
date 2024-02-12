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
    }
}
