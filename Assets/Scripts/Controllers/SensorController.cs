using Controllers.Base;
using Data.Object;
using Utils;

namespace Controllers
{
    public class SensorController : ControllerBase<BsSensor>
    {
        // Controller metadata
        public override string Tag => Tags.SensorTag;
    }
}
