using System;
using System.Collections.Generic;
using System.Linq;
using Brutalsky;
using Controllers;
using Controllers.Player;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class PlayerSystem : MonoBehaviour
    {
        public static PlayerSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Variables
        public Dictionary<string, BsPlayer> ActivePlayers { get; private set; } = new();

        // References
        public GameObject playerPrefab;

        // Functions
        public void FreezeAll()
        {
            foreach (var player in ActivePlayers.Values)
            {
                ((PlayerController)player.InstanceComponent)?.Freeze();
            }
        }

        public void UnfreezeAll()
        {
            foreach (var player in ActivePlayers.Values)
            {
                ((PlayerController)player.InstanceComponent)?.Unfreeze();
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
                playerController.BsObject = player;
                playerController.maxHealth = player.Health;
                playerController.color = player.Color.Tint;
                playerObject.GetComponent<PlayerMovementController>().dummy = player.Dummy;
            }
            else
            {
                // Get reference to existing object and ensure player is reset to full health
                playerObject = player.InstanceObject;
                playerController = playerObject.GetComponent<PlayerController>();
                playerController.Refresh();
            }

            // Select a spawnpoint and move the new player object to it
            var spawnPos = map.SelectSpawn();
            playerObject.transform.position = spawnPos;

            // Note player as active and add it to the active player list
            if (player.Active) return;
            player.InstanceObject = playerObject;
            player.InstanceComponent = playerController;
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

        // Updates
        private void FixedUpdate()
        {
            if (!MapSystem._.IsMapLoaded || MapSystem._.ActiveMap == null) return;
            var mapSize = MapSystem._.ActiveMap.Size;
            foreach (var player in ActivePlayers.Values)
            {
                var position = player.InstanceObject.transform.position;
                var x = Mathf.Abs(position.x);
                var y = Mathf.Abs(position.y);
                if (x > mapSize.x / 2f + 3f || y > mapSize.y / 2f + 3f)
                {
                    ((PlayerController)player.InstanceComponent)?.Kill();
                }
            }
        }
    }
}
