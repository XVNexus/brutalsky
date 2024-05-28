using Controllers.Base;
using Data.Object;
using Utils;

namespace Controllers
{
    public class JointController : ControllerBase<BsJoint>
    {
        // Controller metadata
        public override string Tag => Tags.JointTag;
    }
}
