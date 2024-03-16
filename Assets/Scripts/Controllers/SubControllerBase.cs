using Brutalsky;

namespace Controllers
{
    public abstract class SubControllerBase<TO> : BsBehavior where TO : BsObject
    {
        public abstract string Id { get; }
        public abstract bool IsUnused { get; }

        public ControllerBase<TO> Master { get; set; }
        protected bool IsInitialized { get; private set; }

        protected override void OnStart()
        {
            Master = GetComponent<ControllerBase<TO>>();
            Master.AddSub(this);
            OnInit();
            IsInitialized = true;
        }

        protected abstract void OnInit();

        protected override void OnLoad()
        {
            if (IsUnused)
            {
                Master.RemoveSub(this);
                Destroy(this);
            }
        }
    }
}
