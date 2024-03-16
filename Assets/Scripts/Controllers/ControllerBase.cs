using System.Collections.Generic;
using Brutalsky;

namespace Controllers
{
    public abstract class ControllerBase<TO> : BsBehavior where TO : BsObject
    {
        public TO Object { get; set; }

        public Dictionary<string, SubControllerBase<ControllerBase<TO>, TO>> SubControllers = new();
    }
}
