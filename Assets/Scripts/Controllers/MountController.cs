using Brutalsky.Object;
using Controllers.Base;
using Utils.Constants;

namespace Controllers
{
    public class MountController : ControllerBase<BsMount>
    {
        // Controller metadata
        public override string Tag => Tags.MountTag;
    }
}
