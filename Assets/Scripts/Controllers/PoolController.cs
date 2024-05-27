using Controllers.Base;
using Data.Object;
using Utils.Constants;

namespace Controllers
{
    public class PoolController : ControllerBase<BsPool>
    {
        // Controller metadata
        public override string Tag => Tags.PoolTag;
    }
}
