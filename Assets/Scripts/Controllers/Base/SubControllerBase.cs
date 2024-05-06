using Brutalsky.Base;

namespace Controllers.Base
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
            enabled = true;
            Master.AddSub(this);
            OnInit();
            IsInitialized = true;
        }

        protected virtual void OnInit()
        {
        }

        protected override void OnLoad()
        {
            if (!IsUnused) return;
            enabled = false;
            Master.RemoveSub(this);
        }
    }
}
