using System;
using Brutalsky.Object;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;

namespace Controllers.Goal
{
    public class GoalRedirectController : SubControllerBase<BsGoal>
    {
        // Controller metadata
        public override string Id => "redirect";
        public override bool IsUnused => false;

        // Local variables
        private bool _redirecting;
        private Rigidbody2D _grabbedRigidbody;

        // External references
        public ParticleSystem cParticleSystem;

        // Init functions
        protected override void OnInit()
        {
            // Sync particle system color with goal color
            cParticleSystem.GetComponent<Renderer>().material.color = Master.Object.Color;
        }

        // Event functions
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tags.PlayerTag) || _redirecting) return;
            cParticleSystem.transform.position = other.transform.position;
            cParticleSystem.Play();
            _grabbedRigidbody = other.attachedRigidbody;
            _redirecting = true;
            GameManager._.StartRound(Master.Object.Redirect);
        }

        private void FixedUpdate()
        {
            if (!_redirecting) return;
            _grabbedRigidbody.velocity = MathfExt.MoveToLinear(
                _grabbedRigidbody.velocity, Vector2.zero, 100f * Time.fixedDeltaTime);
        }
    }
}
