using Controllers.Base;
using Data.Object;
using Utils.Constants;

namespace Controllers
{
    public class MountController : ControllerBase<BsMount>
    {
        // Controller metadata
        public override string Tag => Tags.MountTag;
    }
}
