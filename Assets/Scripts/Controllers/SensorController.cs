using Brutalsky.Object;
using Controllers.Base;
using Utils.Constants;

namespace Controllers
{
    public class SensorController : ControllerBase<BsSensor>
    {
        // Controller metadata
        public override string Tag => Tags.SensorTag;
    }
}
