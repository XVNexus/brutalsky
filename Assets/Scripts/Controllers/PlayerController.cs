using Brutalsky.Object;
using Controllers.Base;
using Utils.Constants;

namespace Controllers
{
    public class PlayerController : ControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Tag => Tags.PlayerGTag;
    }
}
