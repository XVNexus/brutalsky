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
        public GameObject pSensor;
        public GameObject pMount;
        public Material aLitMaterial;
        public Material aUnlitMaterial;

        // Utility functions
        public static BsObject GetTemplateObject(string tag)
        {
            return tag switch
            {
                Tags.PlayerPrefix => new BsPlayer(),
                Tags.ShapePrefix => new BsShape(),
                Tags.PoolPrefix => new BsPool(),
                Tags.SensorPrefix => new BsSensor(),
                Tags.MountPrefix => new BsMount(),
                _ => throw Errors.InvalidItem("object tag", tag)
            };
        }

        public static BsAddon GetTemplateAddon(string tag)
        {
            return tag switch
            {
                Tags.JointPrefix => new BsJoint(),
                _ => throw Errors.InvalidItem("addon tag", tag)
            };
        }
    }
}
