using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Base;
using Data;
using Data.Base;
using Extensions;
using JetBrains.Annotations;
using Lcs;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

namespace Systems
{
    public class MapSystem : BsBehavior
    {
        // Static instance
        public static MapSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Config options
        public float backgroundFade;
        public float backgroundField;
        public float maxMapSize;

        // Exposed properties
        public Dictionary<uint, byte[]> MapList { get; } = new();
        [CanBeNull] public BsMap ActiveMap { get; private set; }
        public Dictionary<string, BsPlayer> ActivePlayers { get; } = new();
        public bool MapLoaded { get; private set; }

        // External references
        public GameObject gMapParent;
        public GameObject gPlayerParent;
        public SpriteRenderer gBackgroundPattern;
        public SpriteRenderer gBackgroundMain;
        public SpriteRenderer[] gBackgroundEdges;
        public SpriteRenderer[] gBackgroundCorners;
        public SpriteRenderer[] gBackgroundOobs;
        public Light2D cLight2D;

        // System functions
        public void RegisterPlayer(BsPlayer player)
        {
            if (player.Active) throw Errors.RegisterActivePlayer(player.Id);
            player.Activate(gPlayerParent.transform, Array.Empty<BsObject>());
            ActivePlayers[player.Id] = player;
            EventSystem._.EmitPlayerRegister(player);
            if (MapLoaded)
            {
                SpawnPlayer(player);
            }
        }

        public void UnregisterPlayer(BsPlayer player)
        {
            if (!player.Active) throw Errors.UnregisterInactivePlayer(player.Id);
            player.Deactivate();
            ActivePlayers.Remove(player.Id);
            EventSystem._.EmitPlayerUnregister(player);
        }

        public void SpawnPlayers()
        {
            var playerList = ActivePlayers.Values.ToList();
            while (playerList.Count > 0)
            {
                var index = ResourceSystem.Random.NextInt(playerList.Count);
                var player = playerList[index];
                SpawnPlayer(player, ActiveMap.AllowDummies || player.Type != BsPlayer.TypeDummy);
                playerList.RemoveAt(index);
            }
        }

        public void SpawnPlayer(BsPlayer player, bool visible = true)
        {
            SetPlayerFrozen(player.InstanceObject, !visible);
            var position = visible ? ActiveMap.SelectSpawn() : new Vector2(1e6f, 1e6f);
            player.Health = ActiveMap.PlayerHealth;
            player.InstanceObject.transform.localPosition = position;
            EventSystem._.EmitPlayerSpawn(ActiveMap, player, position, visible);
        }

        public void SpawnPlayer(BsPlayer player, Vector2 position)
        {
            SetPlayerFrozen(player.InstanceObject, false);
            player.Health = ActiveMap.PlayerHealth;
            player.InstanceObject.transform.localPosition = position;
            EventSystem._.EmitPlayerSpawn(ActiveMap, player, position, true);
        }

        public bool GetPlayerFrozen(GameObject playerInstanceObject)
        {
            return !playerInstanceObject.GetComponent<Rigidbody2D>().simulated;
        }

        public void SetPlayersFrozen(bool frozen, bool resetVelocity = false)
        {
            foreach (var player in ActivePlayers.Values)
            {
                SetPlayerFrozen(player.InstanceObject, frozen, resetVelocity);
            }
        }

        public void SetPlayerFrozen(GameObject playerInstanceObject, bool frozen, bool resetVelocity = false)
        {
            var rigidbody = playerInstanceObject.GetComponent<Rigidbody2D>();
            rigidbody.simulated = !frozen;
            playerInstanceObject.GetComponent<CircleCollider2D>().enabled = !frozen;
            if (!resetVelocity) return;
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
        }

        public void RegisterMap(BsMap map)
        {
            EventSystem._.EmitMapPreload(map);
            var document = LcsDocument.Serialize(map);
            MapList[map.Id] = document.Binify(true);
            // TODO: TEMPORARY
            // ResourceSystem._.SaveFile("Maps", $"{map.Author} - {map.Title}", document, LcsDocument.FormatString);
        }

        public void UnregisterMaps()
        {
            EventSystem._.EmitMapsUnload();
            MapList.Clear();
        }

        public void BuildMap(string title, string author)
        {
            BuildMap(GenerateId(title, author));
        }

        public void BuildMap(uint id)
        {
            BuildMap(id > 0 ? LcsDocument.Deserialize<BsMap>(LcsDocument.Parse(MapList[id], true)) : null);
        }

        public void BuildMap([CanBeNull] BsMap map = null)
        {
            map ??= ActiveMap ?? throw Errors.BuildNullMap();
            if (map.PlayArea.width > maxMapSize || map.PlayArea.height > maxMapSize)
                throw Errors.OversizedMap(map.PlayArea.size, maxMapSize);

            // Make sure there is not already an active map
            if (MapLoaded)
            {
                UnbuildMap();
            }

            // Note map as active
            ActiveMap = map;
            MapLoaded = true;

            // Apply config
            CameraSystem._.SetBaseRect(map.PlayArea);
            var backgroundColor = map.BackgroundColor;
            var halfArea = map.PlayArea.size * .5f;
            var halfFade = backgroundFade * .5f;
            var halfField = backgroundField * .5f;
            gBackgroundPattern.color = backgroundColor;
            gBackgroundPattern.transform.parent.localPosition = map.PlayArea.center;
            gBackgroundPattern.size = map.PlayArea.size + backgroundFade * new Vector2(2f, 2f);
            gBackgroundMain.color = backgroundColor.SetAlpha(.25f);
            gBackgroundMain.transform.localScale = map.PlayArea.size;
            for (var i = 0; i < 8; i++)
            {
                var gBackgroundEdge = gBackgroundEdges[i];
                gBackgroundEdge.color = i < 4
                    ? backgroundColor.SetAlpha(.25f)
                    : backgroundColor;
                gBackgroundEdge.transform.localPosition = (i % 4) switch
                {
                    0 => new Vector2(0f, halfArea.y + halfFade),
                    1 => new Vector2(halfArea.x + halfFade, 0f),
                    2 => new Vector2(0f, -halfArea.y - halfFade),
                    3 => new Vector2(-halfArea.x - halfFade, 0f),
                    _ => gBackgroundEdge.transform.localPosition
                };
                gBackgroundEdge.transform.localScale = i % 2 == 0
                    ? new Vector2(map.PlayArea.width, backgroundFade)
                    : new Vector2(map.PlayArea.height, backgroundFade);
            }
            for (var i = 0; i < 8; i++)
            {
                var gBackgroundCorner = gBackgroundCorners[i];
                gBackgroundCorner.color = i < 4
                    ? backgroundColor.SetAlpha(.25f)
                    : backgroundColor;
                gBackgroundCorner.transform.localPosition = (i % 4) switch
                {
                    0 => new Vector2(halfArea.x + halfFade, halfArea.y + halfFade),
                    1 => new Vector2(halfArea.x + halfFade, -halfArea.y - halfFade),
                    2 => new Vector2(-halfArea.x - halfFade, -halfArea.y - halfFade),
                    3 => new Vector2(-halfArea.x - halfFade, halfArea.y + halfFade),
                    _ => gBackgroundCorner.transform.localPosition
                };
                gBackgroundCorner.transform.localScale = new Vector2(backgroundFade, backgroundFade);
            }
            for (var i = 0; i < 4; i++)
            {
                var gBackgroundOob = gBackgroundOobs[i];
                gBackgroundOob.color = backgroundColor;
                gBackgroundOob.transform.localPosition = i switch
                {
                    0 => new Vector2(0f, halfArea.y + backgroundFade + halfField),
                    1 => new Vector2(halfArea.x + backgroundFade + halfField, 0f),
                    2 => new Vector2(0f, -halfArea.y - backgroundFade - halfField),
                    3 => new Vector2(-halfArea.x - backgroundFade - halfField, 0f),
                    _ => gBackgroundOob.transform.localPosition
                };
                gBackgroundOob.transform.localScale = new Vector2(backgroundField, backgroundField);
            }
            cLight2D.color = map.LightingColor.StripAlpha();
            cLight2D.intensity = map.LightingColor.a;
            Physics2D.gravity = GravityToVector(map.GravityDirection, map.GravityStrength);

            // Create all objects
            foreach (var obj in map.Objects.Values)
            {
                CreateObject(obj);
            }
            EventSystem._.EmitMapBuild(ActiveMap);

            // Start the logic system
            Invoke(nameof(InitMatrix), 0f);
        }

        private void InitMatrix()
        {
            ActiveMap.InitMatrix();
        }

        public void UnbuildMap()
        {
            // Make sure there is an active map to unbuild
            if (!MapLoaded) return;

            // Give everything a chance to remove any trash that might get left behind
            EventSystem._.EmitMapCleanup(ActiveMap);

            // Delete all objects and destroy the map container
            foreach (var obj in ActiveMap.Objects.Values)
            {
                DeleteObject(obj);
            }
            ActiveMap.ResetSpawns();

            // Stop the logic system
            ActiveMap.ClearMatrix();

            // Note map as inactive
            EventSystem._.EmitMapUnbuild(ActiveMap);
            ActiveMap = null;
            MapLoaded = false;
        }

        public void CreateObject(BsObject obj)
        {
            if (obj.Active) return;
            var parentTransform = obj.Relatives.Length == 0
                ? gMapParent.transform
                : ActiveMap.Objects[obj.Relatives[0]].InstanceObject.transform;
            var relatedObjects = obj.Relatives.Length > 0
                ? obj.Relatives.Select(id => ActiveMap.Objects[id]).ToArray()
                : Array.Empty<BsObject>();
            obj.Activate(parentTransform, relatedObjects);
        }

        public void DeleteObject(BsObject obj)
        {
            if (!obj.Active) return;
            obj.Deactivate();
        }

        // Event functions
        private void FixedUpdate()
        {
            if (!MapLoaded) return;

            // Update the logic system
            ActiveMap.UpdateMatrix();
        }

        // Utility functions
        public static Vector2 GravityToVector(byte direction, float strength)
        {
            return direction switch
            {
                BsMap.DirectionDown => Vector2.down,
                BsMap.DirectionUp => Vector2.up,
                BsMap.DirectionLeft => Vector2.left,
                BsMap.DirectionRight => Vector2.right,
                _ => Vector2.zero
            } * strength;
        }

        public static uint GenerateId(string title, string author)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes((title + author).GetHashCode()), 0);
        }
    }
}
