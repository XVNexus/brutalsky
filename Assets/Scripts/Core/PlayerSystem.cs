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
        public static PlayerSystem current;
        private void Awake() => current = this;

        // Variables
        public Dictionary<string, BsPlayer> activePlayers { get; private set; } = new();

        // References
        public GameObject playerPrefab;

        // Functions
        public void FreezeAll()
        {
            foreach (var player in activePlayers.Values)
            {
                ((PlayerController)player.instanceComponent).Freeze();
            }
        }

        public void UnfreezeAll()
        {
            foreach (var player in activePlayers.Values)
            {
                ((PlayerController)player.instanceComponent).Unfreeze();
            }
        }

        public void Spawn(BsMap map, [CanBeNull] IEnumerable<BsPlayer> players = null)
        {
            var playerList = (players ?? activePlayers.Values).ToList();
            while (playerList.Count > 0)
            {
                var index = EventSystem.random.NextInt(playerList.Count);
                Spawn(map, playerList[index]);
                playerList.RemoveAt(index);
            }
        }

        public void Spawn(BsMap map, BsPlayer player)
        {
            GameObject playerObject;
            PlayerController playerController;
            if (!player.active)
            {
                // Create new object and apply config
                playerObject = Instantiate(playerPrefab);
                playerController = playerObject.GetComponent<PlayerController>();
                playerController.bsObject = player;
                playerController.maxHealth = player.health;
                playerController.color = player.color.tint;
                playerObject.GetComponent<PlayerMovementController>().dummy = player.dummy;
            }
            else
            {
                // Get reference to existing object and ensure player is reset to full health
                playerObject = player.instanceObject;
                playerController = playerObject.GetComponent<PlayerController>();
                playerController.Refresh();
            }

            // Select a spawnpoint and move the new player object to it
            var spawnPos = map.SelectSpawn();
            playerObject.transform.position = spawnPos;

            // Note player as active and add it to the active player list
            if (player.active) return;
            player.instanceObject = playerObject;
            player.instanceComponent = playerController;
            player.active = true;
            activePlayers[player.id] = player;
        }

        public void Despawn([CanBeNull] IEnumerable<BsPlayer> players = null)
        {
            var playerList = (players ?? activePlayers.Values).ToList();
            foreach (var player in playerList)
            {
                Despawn(player);
            }
        }

        public void Despawn(BsPlayer player)
        {
            // Make sure the player is not already despawned
            if (!player.active) return;

            // Destroy the player object
            Destroy(player.instanceObject);

            // Note player as inactive and remove it from the active player list
            player.instanceObject = null;
            player.active = false;
            activePlayers.Remove(player.id);
        }

        // Updates
        private void FixedUpdate()
        {
            if (!MapSystem.current.mapLoaded) return;
            var mapSize = MapSystem.current.activeMap.size;
            foreach (var player in activePlayers.Values)
            {
                var position = player.instanceObject.transform.position;
                var x = Mathf.Abs(position.x);
                var y = Mathf.Abs(position.y);
                if (x > mapSize.x / 2f + 3f || y > mapSize.y / 2f + 3f)
                {
                    ((PlayerController)player.instanceComponent).Kill();
                }
            }
        }
    }
}
