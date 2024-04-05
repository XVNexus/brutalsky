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

        // External references
        public GameObject playerPrefab;

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

        public void Spawn(BsMap map, [CanBeNull] IEnumerable<BsPlayer> players = null)
        {
            var playerList = (players ?? ActivePlayers.Values).ToList();
            while (playerList.Count > 0)
            {
                var index = EventSystem.Random.NextInt(playerList.Count);
                Spawn(map, playerList[index]);
                playerList.RemoveAt(index);
            }
        }

        public void Spawn(BsMap map, BsPlayer player)
        {
            GameObject playerObject;
            PlayerController playerController;
            if (!player.Active)
            {
                // Create new object and apply config
                playerObject = Instantiate(playerPrefab);
                playerController = playerObject.GetComponent<PlayerController>();
                playerController.Object = player;

                // Apply config and color
                playerController.GetComponent<SpriteRenderer>().color = player.Color.Tint;
            }
            else
            {
                // Get reference to existing object and ensure player is reset to full health
                playerObject = player.InstanceObject;
                playerController = playerObject.GetComponent<PlayerController>();
                playerController.GetSub<PlayerHealthController>("health")?.Refresh();
            }

            // Select a spawnpoint and move the new player object to it
            var spawnPos = map.SelectSpawn();
            playerObject.transform.position = spawnPos;

            // Note player as active and add it to the active player list
            if (player.Active) return;
            player.InstanceObject = playerObject;
            player.InstanceController = playerController;
            player.Active = true;
            ActivePlayers[player.Id] = player;
        }

        public void Despawn([CanBeNull] IEnumerable<BsPlayer> players = null)
        {
            var playerList = (players ?? ActivePlayers.Values).ToList();
            foreach (var player in playerList)
            {
                Despawn(player);
            }
        }

        public void Despawn(BsPlayer player)
        {
            // Make sure the player is not already despawned
            if (!player.Active) return;

            // Destroy the player object
            Destroy(player.InstanceObject);

            // Note player as inactive and remove it from the active player list
            player.InstanceObject = null;
            player.Active = false;
            ActivePlayers.Remove(player.Id);
        }
    }
}
