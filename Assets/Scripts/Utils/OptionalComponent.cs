using JetBrains.Annotations;
using UnityEngine;

namespace Utils
{
    public class OptionalComponent<T> where T : MonoBehaviour
    {
        public T component { get; }
        public bool exists { get; }

        public OptionalComponent([CanBeNull] T component)
        {
            if (component == null) return;
            this.component = component;
            exists = true;
        }
    }
}
