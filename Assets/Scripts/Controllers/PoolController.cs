using Controllers.Base;
using Data.Object;
using Utils;

namespace Controllers
{
    public class PoolController : ControllerBase<BsPool>
    {
        // Controller metadata
        public override string Tag => Tags.PoolTag;
    }
}
