using Brutalsky;

namespace Controllers
{
    public abstract class SubControllerBase<TC, TO> : BsBehavior where TC : ControllerBase<TO> where TO : BsObject
    {
        public TC Master { get; set; }
        protected bool IsInitialized { get; private set; }
        public abstract bool IsUnused { get; }

        protected override void OnStart()
        {
            Master = GetComponent<TC>();
            OnInit();
            IsInitialized = true;
        }

        protected abstract void OnInit();

        protected override void OnLoad()
        {
            if (IsUnused)
            {
                Destroy(this);
            }
        }
    }
}
