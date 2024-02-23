using Brutalsky;
using Brutalsky.Object;

namespace Serializable
{
    public class SrzJoint
    {
        public string id { get; set; }
        public BsTransform transform { get; set; }

        // TODO: MAKE THIS SHIT WORK
        public static SrzJoint Simplify(BsJoint joint)
        {
            return new SrzJoint
            {
                id = joint.id,
                transform = joint.transform,
                
            };
        }

        // TODO: MAKE THIS SHIT WORK
        public BsJoint Expand()
        {
            return new BsJoint
            (
                id,
                transform,
            );
        }
    }
}
