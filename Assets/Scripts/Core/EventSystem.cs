using System;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Core
{
    public class EventSystem : MonoBehaviour
    {
        public static EventSystem current;
        public static Random random;

        private void Awake()
        {
            current = this;
            random = Random.CreateFromIndex((uint)DateTime.UtcNow.Ticks);
        }

        public void TriggerGuiLoad() => OnGuiLoad?.Invoke();
        public event Action OnGuiLoad;
    }
}
