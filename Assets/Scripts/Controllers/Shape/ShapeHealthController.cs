using Brutalsky;
using Controllers.Player;
using UnityEngine;
using Utils;

namespace Controllers.Shape
{
    public class ShapeHealthController : SubControllerBase<BsShape>
    {
        public override string Id => "health";
        public override bool IsUnused => Master.Object.Material.Health == 0f;

        protected override void OnInit()
        {
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!IsInitialized) return;

            // Apply health to player
            if (!other.gameObject.CompareTag(Tags.Player)) return;
            var health = Master.Object.Material.Health;
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (health > 0f)
            {
                playerController.GetSub<PlayerHealthController>("health")?.Heal(health);
            }
            else
            {
                playerController.GetSub<PlayerHealthController>("health")?.Hurt(-health);
            }
        }
    }
}
