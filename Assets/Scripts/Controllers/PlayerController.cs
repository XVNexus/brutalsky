using Brutalsky;
using Controllers.Base;
using Utils.Constants;

namespace Controllers
{
    public class PlayerController : ControllerBase<BsPlayer>
    {
        public override string Tag => Tags.Player;
    }
}
