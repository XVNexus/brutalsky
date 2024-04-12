using Brutalsky;
using Controllers.Base;
using Controllers.Player;
using UnityEngine;
using Utils.Constants;

namespace Controllers.Shape
{
    public class ShapeHealthController : SubControllerBase<BsShape>
    {
        // Controller metadata
        public override string Id => "health";
        public override bool IsUnused => Master.Object.Material.Health == 0f || !Master.Object.Simulated;

        // Event functions
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!IsInitialized) return;

            // Apply health to player
            if (!other.gameObject.CompareTag(Tags.PlayerName)) return;
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
