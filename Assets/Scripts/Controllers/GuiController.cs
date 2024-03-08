using System.Collections.Generic;
using System.Linq;
using Controllers.Gui;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Utils;

namespace Controllers
{
    public class GuiController : MonoBehaviour
    {
        // Constants
        public const string DisabledClass = "bs-disabled";
        public const string PauseMenuId = "pm";

        // Variables
        private Dictionary<string, GuiPane> panes = new();
        private Dictionary<string, string> parentRegistrationHold = new();

        // References
        public VisualElement root;

        // Controls
        public InputAction iEscape;

        // Functions
        public void Escape()
        {
            if (GetVisiblePane()?.Deactivate() ?? false)
            {
                if (!ContainsVisiblePane())
                {
                    TimeSystem.current.Unpause();
                }
            }
            else if (GetPane(PauseMenuId)?.Activate() ?? false)
            {
                TimeSystem.current.Pause();
            }
        }

        [CanBeNull]
        public GuiPane GetVisiblePane()
        {
            return panes.Values.FirstOrDefault(pane => pane.visible && !pane.dummy);
        }

        public bool ContainsVisiblePane()
        {
            return panes.Values.Any(pane => pane.visible && !pane.dummy);
        }

        [CanBeNull]
        public GuiPane GetPane(string id)
        {
            return panes.GetValueOrDefault(id, null);
        }

        public bool RegisterPane(string id, MonoBehaviour controller, string parentId = "")
        {
            if (ContainsPane(id)) return false;

            var pane = new GuiPane(id, GetPaneElement(id), controller);
            panes[id] = pane;
            if (ContainsPane(parentId))
            {
                pane.SetParent(GetPane(parentId));
            }
            else
            {
                parentRegistrationHold[id] = parentId;
            }

            var childrenToRegister = (from childId in parentRegistrationHold.Keys
                where parentRegistrationHold[childId] == id select GetPane(childId)).ToList();
            for (var i = 0; i < childrenToRegister.Count; i++)
            {
                var childToRegister = childrenToRegister[i];
                childToRegister.SetParent(pane);
                parentRegistrationHold.Remove(childToRegister.id);
            }

            return true;
        }

        public bool UnregisterPane(string id)
        {
            if (!ContainsPane(id)) return false;
            panes.Remove(id);
            return true;
        }

        public bool ContainsPane(string id)
        {
            return panes.ContainsKey(id);
        }

        public bool ActivatePane(string id)
        {
            return GetPane(id)?.Activate() ?? false;
        }

        public bool DeactivatePane(string id)
        {
            return GetPane(id)?.Deactivate() ?? false;
        }

        public void RegisterButtons(string paneId, IEnumerable<string> itemIds)
        {
            foreach (var itemId in itemIds)
            {
                RegisterButton(paneId, itemId);
            }
        }

        public void RegisterButton(string paneId, string itemId)
        {
            RegisterButton(GetInputElement<Button>(paneId, itemId), paneId, itemId);
        }

        public void RegisterButton(Button button, string paneId, string itemId)
        {
            button.clicked += () =>
            {
                EventSystem.current.EmitGuiAction(GuiAction.ButtonPress, paneId, itemId);
            };
        }

        public VisualElement GetPaneElement(string paneId)
        {
            return root.Q<VisualElement>($"pane-{paneId}");
        }

        public T GetInputElement<T>(string paneId, string itemId) where T : VisualElement
        {
            var type = typeof(T);
            return type switch
            {
                not null when type == typeof(Button) => root.Q<T>($"{paneId}-btn-{itemId}"),
                not null when type == typeof(Toggle) => root.Q<T>($"{paneId}-tgl-{itemId}"),
                not null when type == typeof(TextField) => root.Q<T>($"{paneId}-txt-{itemId}"),
                not null when type == typeof(IntegerField) => root.Q<T>($"{paneId}-int-{itemId}"),
                not null when type == typeof(FloatField) => root.Q<T>($"{paneId}-flt-{itemId}"),
                _ => throw Errors.InvalidGuiElement()
            };
        }

        // Events
        private void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            panes[""] = new GuiPane("", root, this, true);

            iEscape = EventSystem.current.inputActionAsset.FindAction("Escape");
            iEscape.Enable();

            iEscape.performed += OnIEscape;

            Invoke(nameof(LoadGui), .1f);
        }

        private void LoadGui()
        {
            EventSystem.current.EmitGuiLoad();
        }

        private void OnIEscape(InputAction.CallbackContext context)
        {
            Escape();
        }
    }
}
