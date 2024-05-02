using Controllers.Base;
using Controllers.Player;
using UnityEngine;
using Utils.Constants;

namespace Controllers
{
    public class VoidController : BsBehavior
    {
        // Event functions
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tags.Player)) return;

            // Kill any players that enter the void
            var mHealth = other.GetComponent<PlayerController>().GetSub<PlayerHealthController>("health");
            if (mHealth && mHealth.alive)
            {
                mHealth.Kill();
            }
        }
    }
}
