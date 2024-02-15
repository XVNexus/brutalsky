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

        public void TriggerPlayerDespawn(BsMap map, BsPlayer player) => OnPlayerDespawn?.Invoke(map, player);
        public event Action<BsMap, BsPlayer> OnPlayerDespawn;

        public void TriggerPlayerDie(BsPlayer player) => OnPlayerDie?.Invoke(player);
        public event Action<BsPlayer> OnPlayerDie;

        public void TriggerCameraShake(Vector2 shove, float shake) => OnCameraShake?.Invoke(shove, shake);
        public event Action<Vector2, float> OnCameraShake;
    }
}
