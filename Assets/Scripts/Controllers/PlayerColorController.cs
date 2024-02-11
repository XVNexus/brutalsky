using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Controllers
{
    public class PlayerColorController : MonoBehaviour
    {
        public Color playerColor;

        public Light2D cLight2D;
        public SpriteRenderer cRingSpriteRenderer;
        public ParticleSystem[] cParticleSystems;

        private SpriteRenderer cPlayerSpriteRenderer;

        private void Start()
        {
            cPlayerSpriteRenderer = GetComponent<SpriteRenderer>();

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
