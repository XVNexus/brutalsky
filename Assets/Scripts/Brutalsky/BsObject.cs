using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky
{
    public abstract class BsObject : MonoBehaviour
    {
        public string id { get; set; }
        public Vector2 position { get; set; }
        public float rotation { get; set; }
        [CanBeNull] public GameObject instance { get; private set; }
        public bool active { get; private set; }

        protected BsObject(string id)
        {
            this.id = id;
        }

        protected abstract GameObject _Create();

        public bool Create()
        {
            if (active) return false;
            instance = _Create();
            var instanceTransform = instance.transform;
            instanceTransform.position = new Vector3(position.x, position.y, 0f);
            instanceTransform.rotation = Quaternion.Euler(0f, 0f, rotation);
            active = true;
            return true;
        }

        public bool Destroy()
        {
            if (!active) return false;
            Destroy(instance);
            active = false;
            return true;
        }
    }
}
