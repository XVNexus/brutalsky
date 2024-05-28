using System;
using Controllers.Base;
using Data.Object;
using UnityEngine;
using Utils;

namespace Controllers.Goal
{
    public class GoalRedirectController : SubControllerBase<BsGoal>
    {
        // Controller metadata
        public override string Id => "redirect";
        public override bool IsUnused => false;

        // Local variables
        private bool _redirecting;

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
            _redirecting = true;
            GameManager._.StartRound(Master.Object.Redirect);
        }
    }
}
