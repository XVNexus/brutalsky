using Brutalsky;
using Controllers.Base;
using Controllers.Player;
using UnityEngine;
using Utils.Constants;

namespace Controllers
{
    public class PlayerController : ControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Tag => Tags.PlayerTag;

        // Master functions
        public void Refresh()
        {
            // Reset velocity
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            // Reset health
            var mHealth = GetSub<PlayerHealthController>("health");
            if (mHealth.alive)
            {
                mHealth.health = mHealth.maxHealth;
            }
            else
            {
                mHealth.Revive();
            }

            // Reset boost
            var mMovement = GetSub<PlayerMovementController>("movement");
            if (mMovement)
            {
                mMovement.boostCharge = 0f;
                mMovement.boostCooldown = 0f;
            }
        }
    }
}
