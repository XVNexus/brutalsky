using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class PrefabSystem : MonoBehaviour
    {
        public static PrefabSystem current;
        private void Awake() => current = this;

        public GameObject player;
        public GameObject shape;
        public GameObject pool;
    }
}
