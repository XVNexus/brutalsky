using Brutalsky;
using Controllers.Player;
using UnityEngine;
using Utils;

namespace Controllers.Pool
{
    public class PoolHealthController : SubControllerBase<BsPool>
    {
        public override string Id => "health";
        public override bool IsUnused => Master.Object.Chemical.Health == 0f || !Master.Object.Simulated;

        private void OnTriggerStay2D(Collider2D other)
        {
            // Apply damage to player
            var health = Master.Object.Chemical.Health;
            if (!other.gameObject.CompareTag(Tags.Player) || health == 0f) return;
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (health > 0f)
            {
                playerController.GetSub<PlayerHealthController>("health")?.Heal(health * Time.fixedDeltaTime);
            }
            else
            {
                playerController.GetSub<PlayerHealthController>("health")?.Hurt(-health * Time.fixedDeltaTime);
            }
        }
    }
}
