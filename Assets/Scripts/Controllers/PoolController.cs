using Brutalsky;
using Controllers.Base;
using Utils.Constants;

namespace Controllers
{
    public class PoolController : ControllerBase<BsPool>
    {
        // Controller metadata
        public override string Tag => Tags.PoolTag;
    }
}
