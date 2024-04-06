using System.Collections.Generic;
using System.Linq;
using Brutalsky;
using Controllers;
using Controllers.Player;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class PlayerSystem : BsBehavior
    {
        // Static instance
        public static PlayerSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Local variables
        public Dictionary<string, BsPlayer> ActivePlayers { get; private set; } = new();

        // System functions
        public void FreezeAll()
        {
            foreach (var player in ActivePlayers.Values)
            {
                ((PlayerController)player.InstanceController)?.GetSub<PlayerMovementController>("movement")?.Freeze();
            }
        }

        public void UnfreezeAll()
        {
            foreach (var player in ActivePlayers.Values)
            {
                ((PlayerController)player.InstanceController)?.GetSub<PlayerMovementController>("movement")?.Unfreeze();
            }
        }

        public void SpawnAll(BsMap map, [CanBeNull] IEnumerable<BsPlayer> players = null)
        {
            var playerList = (players ?? ActivePlayers.Values).ToList();
            while (playerList.Count > 0)
            {
                var index = ResourceSystem.Random.NextInt(playerList.Count);
                Spawn(map, playerList[index]);
                playerList.RemoveAt(index);
            }
        }

        public void DespawnAll([CanBeNull] IEnumerable<BsPlayer> players = null)
        {
            var playerList = (players ?? ActivePlayers.Values).ToList();
            foreach (var player in playerList)
            {
                Despawn(player);
            }
        }

        public bool Spawn(BsMap map, BsPlayer player)
        {
            if (!MapSystem._.IsMapLoaded) return false;
            if (!player.Active)
            {
                // Spawn new player
                var instanceObject = Instantiate(player.Prefab);
                var instanceController = player.Init(instanceObject, map);
                player.Activate(instanceObject, instanceController);
                ActivePlayers[player.Id] = player;
                instanceObject.transform.position = map.SelectSpawn();
            }
            else
            {
                // Respawn existing player
                player.Refresh(map);
                player.InstanceObject.transform.position = map.SelectSpawn();
            }
            return true;
        }

        public bool Despawn(BsPlayer player)
        {
            if (!player.Active || !MapSystem._.IsMapLoaded) return false;
            Destroy(player.InstanceObject);
            player.Deactivate();
            ActivePlayers.Remove(player.Id);
            return true;
        }
    }
}
