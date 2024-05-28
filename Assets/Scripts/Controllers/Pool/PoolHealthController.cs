using Controllers.Base;
using Controllers.Player;
using Data.Object;
using UnityEngine;
using Utils;

namespace Controllers.Pool
{
    public class PoolHealthController : SubControllerBase<BsPool>
    {
        // Controller metadata
        public override string Id => "health";
        public override bool IsUnused => Master.Object.Health == 0f;

        // Event functions
        private void OnTriggerStay2D(Collider2D other)
        {
            // Apply damage to player
            var health = Master.Object.Health;
            if (!other.gameObject.CompareTag(Tags.PlayerTag)) return;
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
