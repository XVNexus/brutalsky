using UnityEngine;

namespace Core
{
    public class MapSystem : MonoBehaviour
    {
        public static MapSystem current;
        private void Awake() => current = this;

        
    }
}
