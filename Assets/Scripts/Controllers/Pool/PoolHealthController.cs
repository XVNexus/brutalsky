using Brutalsky;
using UnityEngine;
using Utils;

namespace Controllers.Pool
{
    public class PoolHealthController : SubControllerBase<PoolController, BsPool>
    {
        public override bool IsUnused => Master.Object.Chemical.Health == 0f;

        protected override void OnInit()
        {
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            // Apply damage to player
            var damage = Master.Object.Chemical.Health;
            if (!other.gameObject.CompareTag(Tags.Player) || damage == 0f) return;
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (damage > 0f)
            {
                playerController.Heal(damage * Time.fixedDeltaTime);
            }
            else
            {
                playerController.Hurt(-damage * Time.fixedDeltaTime);
            }
        }
    }
}
