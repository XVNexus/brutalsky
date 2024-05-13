using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Base;
using JetBrains.Annotations;
using UnityEngine;
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

        // Config options
        public string disabledClass;
        public string pauseMenuId;

        // Local variables
        private VisualElement _root;
        private readonly Dictionary<string, GuiPane> _panes = new();
        private readonly Dictionary<string, string> _parentRegistrationHold = new();
        private readonly Dictionary<(string, string), (Button, Action)> _buttons = new();

        // External references
        public UIDocument cUIDocument;

        // Init functions
        protected override void OnStart()
        {
            _root = cUIDocument.rootVisualElement;
            _panes[""] = new GuiPane("", _root, this, true);

            EventSystem._.SetInputAction("Gui", "Escape", _ => EscapeOne());
        }

        // System functions
        public void EscapeOne()
        {
            if (DeactivatePane())
            {
                if (!ContainsVisiblePane())
                {
                    TimeSystem._.Unpause();
                }
            }
            else
            {
                ActivatePane(pauseMenuId);
                TimeSystem._.Pause();
            }
        }

        public void EscapeAll()
        {
            if (!ContainsVisiblePane()) return;
            while (DeactivatePane()) { }
            TimeSystem._.Unpause();
        }

        public bool ActivatePane(string id)
        {
            return GetPane(id).Activate();
        }

        public bool DeactivatePane(string id)
        {
            return GetPane(id).Deactivate();
        }

        public bool DeactivatePane()
        {
            return GetVisiblePane()?.Deactivate() ?? false;
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
            if (!ContainsPane(id)) throw Errors.NoItemFound("gui pane", id);
            return _panes[id];
        }

        public bool ContainsPane(string id)
        {
            return _panes.ContainsKey(id);
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

        public void RegisterButton(string paneId, string itemId, Action onClick)
        {
            RegisterButton(GetInputElement<Button>(paneId, itemId), paneId, itemId, onClick);
        }

        public void RegisterButton(Button button, string paneId, string itemId, Action onClick)
        {
            button.clicked += onClick;
            _buttons[(paneId, itemId)] = (button, onClick);
        }

        public void UnregisterButton(string paneId, string itemId)
        {
            var key = (paneId, itemId);
            if (!_buttons.ContainsKey(key)) return;
            var button = _buttons[key];
            button.Item1.clicked -= button.Item2;
            _buttons.Remove(key);
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
                _ => throw Errors.InvalidItem("gui element type", type)
            };
        }
    }
}
