using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Controllers.Gui
{
    public class GuiPane
    {
        [CanBeNull] public GuiPane parent { get; private set; }
        public Dictionary<string, GuiPane> children { get; } = new();

        public string id { get; set; }
        public VisualElement element { get; set; }
        public MonoBehaviour controller { get; set; }
        public bool active { get; private set; }
        public bool visible { get; private set; }

        public GuiPane(string id, VisualElement element, MonoBehaviour controller)
        {
            this.id = id;
            this.element = element;
            this.controller = controller;
            if (element.ClassListContains(GuiController.DisabledClass))
            {
                Hide();
            }
            else
            {
                active = true;
                Show();
            }
        }

        public void SetParent([CanBeNull] GuiPane pane)
        {
            parent?.RemoveChild(this);
            pane?.AddChild(this);
            parent = pane;
        }

        public bool AddChild(GuiPane pane)
        {
            if (ContainsChild(id)) return false;
            children[id] = pane;
            pane.parent?.RemoveChild(pane);
            pane.parent = this;
            return true;
        }

        public bool RemoveChild(GuiPane pane)
        {
            return RemoveChild(pane.id);
        }

        public bool RemoveChild(string id)
        {
            if (!ContainsChild(id)) return false;
            var pane = children[id];
            pane.parent = null;
            children.Remove(id);
            return true;
        }

        public bool ContainsChild(GuiPane pane)
        {
            return ContainsChild(pane.id);
        }

        public bool ContainsChild(string id)
        {
            return children.ContainsKey(id);
        }

        public bool Activate()
        {
            if (active || parent is { active: false }) return false;
            if (parent != null)
            {
                parent.Hide();
                foreach (var child in parent.children.Values)
                {
                    child.Deactivate();
                }
            }
            Show();
            active = true;
            return true;
        }

        public bool Deactivate()
        {
            if (!active) return false;
            Hide();
            parent?.Show();
            active = false;
            return true;
        }

        private void Show()
        {
            element.RemoveFromClassList(GuiController.DisabledClass);
            visible = true;
        }

        private void Hide()
        {
            element.AddToClassList(GuiController.DisabledClass);
            visible = false;
        }
    }
}
