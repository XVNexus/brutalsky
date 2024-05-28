using Controllers.Base;
using Controllers.Player;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class VoidController : BsBehavior
    {
        // Event functions
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tags.PlayerTag)) return;

            // Kill any players that enter the void
            var mHealth = other.GetComponent<PlayerController>().GetSub<PlayerHealthController>("health");
            if (mHealth && mHealth.Alive)
            {
                mHealth.Kill();
            }
        }
    }
}
