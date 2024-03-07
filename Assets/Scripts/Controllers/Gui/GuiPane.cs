using System.Collections.Generic;
using System.Linq;
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
        public bool dummy { get; private set; }

        public GuiPane(string id, VisualElement element, MonoBehaviour controller, bool dummy = false)
        {
            this.id = id;
            this.element = element;
            this.controller = controller;
            if (!dummy) return;
            this.dummy = true;
            active = true;
            visible = true;
        }

        public void SetParent([CanBeNull] GuiPane pane)
        {
            parent?.RemoveChild(this);
            pane?.AddChild(this);
            parent = pane;
        }

        public bool AddChild(GuiPane pane)
        {
            if (ContainsChild(pane.id)) return false;
            children[pane.id] = pane;
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
            if (dummy || active || parent is { active: false }) return false;
            if (parent != null)
            {
                parent.Hide();
                foreach (var child in parent.children.Values.Where(child => child.id != id))
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
            if (dummy || !active) return false;
            Hide();
            parent?.Show();
            active = false;
            return true;
        }

        private void Show()
        {
            if (dummy) return;
            element.RemoveFromClassList(GuiController.DisabledClass);
            visible = true;
        }

        private void Hide()
        {
            if (dummy) return;
            element.AddToClassList(GuiController.DisabledClass);
            visible = false;
        }
    }
}
