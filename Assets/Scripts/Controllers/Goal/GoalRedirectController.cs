using Brutalsky.Object;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;

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

        // Module functions
        public void ActivateRedirect()
        {
            if (_redirecting) return;
            cParticleSystem.Play();
            GameManager._.StartRound(Master.Object.Redirect);
            _redirecting = true;
        }

        // Event functions
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tags.PlayerTag)) return;
            ActivateRedirect();
        }
    }
}
