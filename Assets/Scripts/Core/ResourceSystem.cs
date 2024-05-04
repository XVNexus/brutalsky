using System;
using Brutalsky.Addon;
using Brutalsky.Base;
using Brutalsky.Object;
using Controllers.Base;
using UnityEngine;
using Utils.Constants;
using Random = Unity.Mathematics.Random;

namespace Core
{
    public class ResourceSystem : BsBehavior
    {
        // Static instance
        public static ResourceSystem _ { get; private set; }
        public static Random Random;
        public static string DataPath;

        private void Awake()
        {
            _ = this;
            Random = Random.CreateFromIndex((uint)DateTime.UtcNow.Ticks);
            DataPath = Application.persistentDataPath;
        }

        // External references
        public GameObject pPlayer;
        public GameObject pShape;
        public GameObject pPool;
        public Material aLitMaterial;
        public Material aUnlitMaterial;

        // Utility functions
        public static BsObject GetTemplateObject(string tag)
        {
            return tag switch
            {
                Tags.PlayerLTag => new BsPlayer(),
                Tags.ShapeLTag => new BsShape(),
                Tags.PoolLTag => new BsPool(),
                _ => throw Errors.InvalidTag("object", tag)
            };
        }

        public static BsAddon GetTemplateAddon(string tag)
        {
            return tag switch
            {
                Tags.JointLTag => new BsJoint(),
                _ => throw Errors.InvalidTag("addon", tag)
            };
        }
    }
}
