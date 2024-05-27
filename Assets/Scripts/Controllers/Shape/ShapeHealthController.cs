using Controllers.Base;
using Controllers.Player;
using Data.Object;
using UnityEngine;
using Utils.Constants;

namespace Controllers.Shape
{
    public class ShapeHealthController : SubControllerBase<BsShape>
    {
        // Controller metadata
        public override string Id => "health";
        public override bool IsUnused => Master.Object.Health == 0f;

        // Event functions
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!IsInitialized) return;

            // Apply health to player
            if (!other.gameObject.CompareTag(Tags.PlayerTag)) return;
            var health = Master.Object.Health;
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
