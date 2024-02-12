namespace Brutalsky
{
    public class BsMaterial
    {
        public bool dynamic { get; }
        public float friction { get; }
        public float restitution { get; }
        public float adhesion { get; }
        public float density { get; }

        public BsMaterial(float friction, float restitution, float adhesion)
        {
            dynamic = false;
            this.friction = friction;
            this.restitution = restitution;
            this.adhesion = adhesion;
            density = 1f;
        }

        public BsMaterial(float friction, float restitution, float adhesion, float density)
        {
            dynamic = true;
            this.friction = friction;
            this.restitution = restitution;
            this.adhesion = adhesion;
            this.density = density;
        }
    }
}
