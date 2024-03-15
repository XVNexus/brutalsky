using Brutalsky;
using UnityEngine;
using Utils;

namespace Controllers.Shape
{
    public class ShapeHealthController : SubControllerBase<ShapeController, BsShape>
    {
        public override bool IsUnused => Master.Object.Material.Health == 0f;

        protected override void OnInit()
        {
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!IsInitialized) return;

            // Apply health to player
            if (!other.gameObject.CompareTag(Tags.Player)) return;
            var damage = Master.Object.Material.Health;
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (damage > 0f)
            {
                playerController.Heal(damage);
            }
            else
            {
                playerController.Hurt(-damage);
            }
        }
    }
}
