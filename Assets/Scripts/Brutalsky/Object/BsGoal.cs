using System;
using Brutalsky.Base;
using Brutalsky.Logic;
using Controllers;
using Controllers.Base;
using Controllers.Goal;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Object
{
    public class BsGoal : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pGoal;
        public override string Tag => Tags.GoalPrefix;
        public override bool HasLogic => true;

        public float Size { get; set; }
        public Color Color { get; set; }
        public string MapTitle { get; set; }
        public string MapAuthor { get; set; }

        public BsGoal(string id, ObjectTransform transform, bool simulated,
            float size, Color color, string mapTitle, string mapAuthor)
            : base(id, transform, ObjectLayer.Midground, simulated)
        {
            Size = size;
            Color = color;
            MapTitle = mapTitle;
            MapAuthor = mapAuthor;
        }

        public BsGoal()
        {
        }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<GoalController>();
            controller.Object = this;

            // Apply config
            if (!Simulated)
            {
                UnityEngine.Object.Destroy(gameObject.GetComponent<CircleCollider2D>());
            }

            // Apply size and position
            gameObject.transform.localScale = Vector2.one * Size;
            gameObject.transform.localPosition = Transform.Position;

            return controller;
        }

        protected override BsNode _RegisterLogic()
        {
            var redirectController = ((GoalController)InstanceController).GetSub<GoalRedirectController>("redirect");
            return new BsNode(new float[1], Array.Empty<float>(), (inputs, _) =>
            {
                if (BsMatrix.ToBool(inputs[0]))
                {
                    redirectController.ActivateRedirect();
                }
                return Array.Empty<float>();
            });
        }

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(LcsType.Float, Size),
                new(LcsType.Color, Color),
                new(LcsType.String, MapTitle),
                new(LcsType.String, MapAuthor)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            Size = (float)props[0].Value;
            Color = (Color)props[1].Value;
            MapTitle = (string)props[2].Value;
            MapAuthor = (string)props[3].Value;
        }
    }
}
