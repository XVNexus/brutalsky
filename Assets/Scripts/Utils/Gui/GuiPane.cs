using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utils.Gui
{
    public class GuiPane
    {
        [CanBeNull] public GuiPane Parent { get; private set; }
        public Dictionary<string, GuiPane> Children { get; } = new();

        public string Id { get; set; }
        public VisualElement Element { get; set; }
        public MonoBehaviour Controller { get; set; }
        public bool Active { get; private set; }
        public bool Visible { get; private set; }
        public bool Dummy { get; private set; }

        public GuiPane(string id, VisualElement element, MonoBehaviour controller, bool dummy = false)
        {
            Id = id;
            Element = element;
            Controller = controller;
            if (!dummy) return;
            Dummy = true;
            Active = true;
            Visible = true;
        }

        public void SetParent([CanBeNull] GuiPane pane)
        {
            Parent?.RemoveChild(this);
            pane?.AddChild(this);
            Parent = pane;
        }

        public bool AddChild(GuiPane pane)
        {
            if (ContainsChild(pane.Id)) return false;
            Children[pane.Id] = pane;
            pane.Parent?.RemoveChild(pane);
            pane.Parent = this;
            return true;
        }

        public bool RemoveChild(GuiPane pane)
        {
            return RemoveChild(pane.Id);
        }

        public bool RemoveChild(string id)
        {
            if (!ContainsChild(id)) return false;
            var pane = Children[id];
            pane.Parent = null;
            Children.Remove(id);
            return true;
        }

        public bool ContainsChild(GuiPane pane)
        {
            return ContainsChild(pane.Id);
        }

        public bool ContainsChild(string id)
        {
            return Children.ContainsKey(id);
        }

        public bool Activate()
        {
            if (Dummy || Active || Parent is { Active: false }) return false;
            if (Parent != null)
            {
                Parent.Hide();
                foreach (var child in Parent.Children.Values.Where(child => child.Id != Id))
                {
                    child.Deactivate();
                }
            }
            Show();
            Active = true;
            return true;
        }

        public bool Deactivate()
        {
            if (Dummy || !Active) return false;
            Hide();
            Parent?.Show();
            Active = false;
            return true;
        }

        private void Show()
        {
            if (Dummy) return;
            Element.RemoveFromClassList(GuiSystem._.disabledClass);
            Visible = true;
        }

        private void Hide()
        {
            if (Dummy) return;
            Element.AddToClassList(GuiSystem._.disabledClass);
            Visible = false;
        }
    }
}
