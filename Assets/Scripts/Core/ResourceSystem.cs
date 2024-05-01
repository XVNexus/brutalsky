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

        // System functions
        public BsObject GetTemplateObject(char objectPrefix)
        {
            return objectPrefix switch
            {
                Tags.PlayerSym => new BsPlayer(),
                Tags.ShapeSym => new BsShape(),
                Tags.PoolSym => new BsPool(),
                _ => throw Errors.InvalidObjectOrAddonTag("object", objectPrefix)
            };
        }

        public BsAddon GetTemplateAddon(char addonPrefix)
        {
            return addonPrefix switch
            {
                Tags.JointSym => new BsJoint(),
                _ => throw Errors.InvalidObjectOrAddonTag("addon", addonPrefix)
            };
        }
    }
}
