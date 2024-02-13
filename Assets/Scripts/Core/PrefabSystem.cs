using UnityEngine;

namespace Core
{
    public class PrefabSystem : MonoBehaviour
    {
        public static PrefabSystem current;
        private void Awake() => current = this;

        // References
        public GameObject player;
        public GameObject shape;
        public GameObject pool;
    }
}
