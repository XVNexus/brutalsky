namespace Brutalsky.Base
{
    public abstract class BsNode
    {
        public abstract string Id { get; }

        public abstract float[] Compute(float[] inputs);
    }
}
