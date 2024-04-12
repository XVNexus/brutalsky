using System;
using Brutalsky;
using Brutalsky.Base;
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
        public GameObject playerPrefab;
        public GameObject shapePrefab;
        public GameObject poolPrefab;
        public Material litMaterial;
        public Material unlitMaterial;

        // System functions
        public BsObject GetTemplateObject(char objectPrefix)
        {
            return objectPrefix switch
            {
                Tags.PlayerPrefix => new BsPlayer(),
                Tags.ShapePrefix => new BsShape(),
                Tags.PoolPrefix => new BsPool(),
                _ => throw Errors.InvalidObjectOrAddonTag("object", objectPrefix)
            };
        }

        public BsAddon GetTemplateAddon(char addonPrefix)
        {
            return addonPrefix switch
            {
                Tags.JointPrefix => new BsJoint(),
                _ => throw Errors.InvalidObjectOrAddonTag("addon", addonPrefix)
            };
        }
    }
}
