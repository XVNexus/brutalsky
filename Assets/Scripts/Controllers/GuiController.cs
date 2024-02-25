using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace Controllers
{
    public class GuiController : MonoBehaviour
    {
        // References
        public VisualElement root;

        // Functions
        public VisualElement GetPane(string paneId)
        {
            return root.Q<VisualElement>($"pane-{paneId}");
        }

        public T GetElement<T>(string paneId, string itemId) where T : VisualElement
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

            EventSystem.current.TriggerGuiLoad();
        }
    }
}
