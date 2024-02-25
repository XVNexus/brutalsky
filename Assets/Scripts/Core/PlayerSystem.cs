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
                player.instanceObject.GetComponent<PlayerController>().Freeze();
            }
        }

        public void UnfreezeAll()
        {
            foreach (var player in activePlayers.Values)
            {
                player.instanceObject.GetComponent<PlayerController>().Unfreeze();
            }
        }

        public void Spawn(IEnumerable<BsPlayer> players, BsMap map)
        {
            var playerList = players.ToList();
            while (playerList.Count > 0)
            {
                var index = EventSystem.random.NextInt(playerList.Count);
                Spawn(playerList[index], map);
                playerList.RemoveAt(index);
            }
        }

        public void Spawn(BsPlayer player, BsMap map)
        {
            GameObject playerObject;
            if (!player.active)
            {
                // Create new object and apply config
                playerObject = Instantiate(playerPrefab);
                var playerController = playerObject.GetComponent<PlayerController>();
                playerController.bsObject = player;
                playerController.maxHealth = player.health;
                playerController.color = player.color.tint;
                if (player.dummy)
                {
                    playerObject.GetComponent<PlayerMovementController>().movementForce = 0f;
                }
            }
            else
            {
                // Get reference to existing object and ensure player is reset to full health
                playerObject = player.instanceObject;
                playerObject.GetComponent<PlayerController>().Revive();
            }

            // Select a spawnpoint and move the new player object to it
            var spawnPos = map.SelectSpawn();
            playerObject.transform.position = spawnPos;

            // Note player as active and add it to the active player list
            if (player.active) return;
            player.instanceObject = playerObject;
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
    }
}
