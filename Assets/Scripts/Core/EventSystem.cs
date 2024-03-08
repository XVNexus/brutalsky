using System;
using Controllers.Gui;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = Unity.Mathematics.Random;

namespace Core
{
    public class EventSystem : MonoBehaviour
    {
        public static EventSystem current;
        public static Random random;
        public static string dataPath;

        private void Awake()
        {
            current = this;
            random = Random.CreateFromIndex((uint)DateTime.UtcNow.Ticks);
            dataPath = Application.persistentDataPath;
        }

        // References
        public InputActionAsset inputActionAsset;

        // Events
        public void EmitGuiLoad() => OnGuiLoad?.Invoke();
        public event Action OnGuiLoad;

        public void EmitGuiAction(GuiAction action, string paneId, string itemId) => OnGuiAction?.Invoke(action, paneId, itemId);
        public event Action<GuiAction, string, string> OnGuiAction;
    }
}
