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
        public override string Tag => Tags.Player;

        // Master functions
        public void Refresh()
        {
            // Reset velocity
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            // Reset health
            var _mHealth = GetSub<PlayerHealthController>("health");
            if (_mHealth.alive)
            {
                _mHealth.health = _mHealth.maxHealth;
            }
            else
            {
                _mHealth.Revive();
            }

            // Reset boost
            var _mMovement = GetSub<PlayerMovementController>("movement");
            if (_mMovement)
            {
                _mMovement.boostCharge = 0f;
                _mMovement.boostCooldown = 0f;
            }
        }
    }
}
