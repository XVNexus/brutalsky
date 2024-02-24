using System;
using Brutalsky;
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

        public void TriggerMapLoad(BsMap map) => OnMapLoad?.Invoke(map);
        public event Action<BsMap> OnMapLoad;

        public void TriggerMapUnload(BsMap map) => OnMapUnload?.Invoke(map);
        public event Action<BsMap> OnMapUnload;

        public void TriggerPlayerSpawn(BsMap map, BsPlayer player) => OnPlayerSpawn?.Invoke(map, player);
        public event Action<BsMap, BsPlayer> OnPlayerSpawn;

        public void TriggerPlayerDespawn(BsPlayer player) => OnPlayerDespawn?.Invoke(player);
        public event Action<BsPlayer> OnPlayerDespawn;
    }
}
