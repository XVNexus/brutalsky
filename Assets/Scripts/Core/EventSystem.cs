using System;
using Controllers.Base;
using UnityEngine.InputSystem;
using Utils.Gui;

namespace Core
{
    public class EventSystem : BsBehavior
    {
        // Static instance
        public static EventSystem _ { get; private set; }
        private void Awake() => _ = this;

        // External references
        public InputActionAsset inputActionAsset;

        // Event functions
        public void EmitGuiAction(GuiAction action, string paneId, string itemId) => OnGuiAction?.Invoke(action, paneId, itemId);
        public event Action<GuiAction, string, string> OnGuiAction;
    }
}
