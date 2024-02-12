using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Controllers
{
    public class PlayerColorController : MonoBehaviour
    {
        // Settings
        public Color playerColor;

        // References
        public Light2D cLight2D;
        public SpriteRenderer cRingSpriteRenderer;
        public ParticleSystem[] cParticleSystems;
        private SpriteRenderer cPlayerSpriteRenderer;

        // Events
        private void Start()
        {
            cPlayerSpriteRenderer = GetComponent<SpriteRenderer>();

            // Set all colored items to match the player color
            cPlayerSpriteRenderer.color = playerColor;
            cLight2D.color = playerColor;
            cRingSpriteRenderer.color = playerColor;
            foreach (var cParticleSystem in cParticleSystems)
            {
                var psMain = cParticleSystem.main;
                psMain.startColor = playerColor;
            }
        }
    }
}
