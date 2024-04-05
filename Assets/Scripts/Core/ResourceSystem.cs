using System;
using Brutalsky;
using Brutalsky.Base;
using UnityEngine;
using Utils.Constants;
using Random = Unity.Mathematics.Random;

namespace Core
{
    public class ResourceSystem : MonoBehaviour
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
        public BsObject GetTemplateObject(string objectTag)
        {
            return objectTag switch
            {
                Tags.Player => new BsPlayer(),
                Tags.Shape => new BsShape(),
                Tags.Pool => new BsPool(),
                _ => throw Errors.InvalidTag("object", objectTag)
            };
        }

        public BsAddon GetTemplateAddon(string addonTag)
        {
            return addonTag switch
            {
                Tags.Joint => new BsJoint(),
                _ => throw Errors.InvalidTag("addon", addonTag)
            };
        }
    }
}
