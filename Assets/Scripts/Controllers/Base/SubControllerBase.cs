using Data.Base;

namespace Controllers.Base
{
    public abstract class SubControllerBase<T> : BsBehavior where T : BsObject
    {
        public abstract string Id { get; }
        public abstract bool IsUnused { get; }

        public ControllerBase<T> Master { get; set; }
        protected bool IsInitialized { get; private set; }

        protected override void OnStart()
        {
            Master = GetComponent<ControllerBase<T>>();
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
