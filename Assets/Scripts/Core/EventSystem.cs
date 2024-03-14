using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Gui;
using Random = Unity.Mathematics.Random;

namespace Core
{
    public class EventSystem : MonoBehaviour
    {
        public static EventSystem _ { get; private set; }
        public static Random Random;
        public static string DataPath;

        private void Awake()
        {
            _ = this;
            Random = Random.CreateFromIndex((uint)DateTime.UtcNow.Ticks);
            DataPath = Application.persistentDataPath;
        }

        // References
        public InputActionAsset inputActionAsset;

        // Events
        private void Start()
        {
            Invoke(nameof(EmitLoad), .1f);
        }

        public void EmitLoad() => OnLoad?.Invoke();
        public event Action OnLoad;

        public void EmitGuiAction(GuiAction action, string paneId, string itemId) => OnGuiAction?.Invoke(action, paneId, itemId);
        public event Action<GuiAction, string, string> OnGuiAction;
    }
}
