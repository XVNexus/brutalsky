using System.Collections.Generic;
using System.Linq;
using Controllers.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Utils.Constants;
using Utils.Gui;

namespace Core
{
    public class GuiSystem : BsBehavior
    {
        // Static instance
        public static GuiSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Local constants
        public const string DisabledClass = "bs-disabled";
        public const string PauseMenuId = "pm";

        // Local variables
        private VisualElement _root;
        private readonly Dictionary<string, GuiPane> _panes = new();
        private readonly Dictionary<string, string> _parentRegistrationHold = new();

        // External references
        public UIDocument cUIDocument;

        // Player input
        public InputAction iEscape;

        // Init functions
        protected override void OnStart()
        {
            _root = cUIDocument.rootVisualElement;
            _panes[""] = new GuiPane("", _root, this, true);

            iEscape = EventSystem._.aInputAction.FindAction("Escape");
            iEscape.Enable();

            iEscape.performed += OnIEscape;
        }

        // System functions
        public void Escape()
        {
            if (GetVisiblePane()?.Deactivate() ?? false)
            {
                if (!ContainsVisiblePane())
                {
                    TimeSystem._.Unpause();
                }
            }
            else if (GetPane(PauseMenuId)?.Activate() ?? false)
            {
                TimeSystem._.Pause();
            }
        }

        [CanBeNull]
        public GuiPane GetVisiblePane()
        {
            return _panes.Values.FirstOrDefault(pane => pane.Visible && !pane.Dummy);
        }

        public bool ContainsVisiblePane()
        {
            return _panes.Values.Any(pane => pane.Visible && !pane.Dummy);
        }

        [CanBeNull]
        public GuiPane GetPane(string id)
        {
            return _panes.GetValueOrDefault(id, null);
        }

        public bool RegisterPane(string id, MonoBehaviour controller, string parentId = "")
        {
            if (ContainsPane(id)) return false;

            var pane = new GuiPane(id, GetPaneElement(id), controller);
            _panes[id] = pane;
            if (ContainsPane(parentId))
            {
                pane.SetParent(GetPane(parentId));
            }
            else
            {
                _parentRegistrationHold[id] = parentId;
            }

            var childrenToRegister = (from childId in _parentRegistrationHold.Keys
                where _parentRegistrationHold[childId] == id select GetPane(childId)).ToList();
            for (var i = 0; i < childrenToRegister.Count; i++)
            {
                var childToRegister = childrenToRegister[i];
                childToRegister.SetParent(pane);
                _parentRegistrationHold.Remove(childToRegister.Id);
            }

            return true;
        }

        public bool UnregisterPane(string id)
        {
            if (!ContainsPane(id)) return false;
            _panes.Remove(id);
            return true;
        }

        public bool ContainsPane(string id)
        {
            return _panes.ContainsKey(id);
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
                EventSystem._.EmitGuiAction(GuiAction.ButtonPress, paneId, itemId);
            };
        }

        public VisualElement GetPaneElement(string paneId)
        {
            return _root.Q<VisualElement>($"pane-{paneId}");
        }

        public T GetInputElement<T>(string paneId, string itemId) where T : VisualElement
        {
            var type = typeof(T);
            return type switch
            {
                not null when type == typeof(Button) => _root.Q<T>($"{paneId}-btn-{itemId}"),
                not null when type == typeof(Toggle) => _root.Q<T>($"{paneId}-tgl-{itemId}"),
                not null when type == typeof(TextField) => _root.Q<T>($"{paneId}-txt-{itemId}"),
                not null when type == typeof(IntegerField) => _root.Q<T>($"{paneId}-int-{itemId}"),
                not null when type == typeof(FloatField) => _root.Q<T>($"{paneId}-flt-{itemId}"),
                _ => throw Errors.InvalidGuiElementType()
            };
        }

        // Event functions
        private void OnIEscape(InputAction.CallbackContext context)
        {
            Escape();
        }
    }
}
