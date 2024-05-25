using Brutalsky.Scripts.Data;

namespace Brutalsky.Scripts.Controllers;

public interface IObjectController
{
    public BsObject Source { get; set; }

    public void Init();
}
