using Brutalsky;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using Utils;

namespace Controllers
{
    public class PlayerController : ControllerBase<BsPlayer>
    {
        public Color color;

        public Light2D cLight2D;
        public SpriteRenderer cRingSpriteRenderer;
        public ParticleSystem[] cParticleSystems;
        private SpriteRenderer _cPlayerSpriteRenderer;
        private Rigidbody2D _cRigidbody2D;
        private CircleCollider2D _cCircleCollider2D;
    }
}
