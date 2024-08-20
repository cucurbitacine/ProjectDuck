using UnityEngine;

namespace Game.VFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class OutlineEffect : MonoBehaviour
    {
        [field: SerializeField] public bool Outlined { get; private set; }
        
        [Header("Settings")]
        [SerializeField] private Color outlineColor = Color.cyan;
        [Range(0f, 1f)]
        [SerializeField] private float outlineAlpha = 0.95f;
        [Min(0f)]
        [SerializeField] private float outlineWidth = 0.004f;
        
        private SpriteRenderer _sprite;
        private Material _material;
        
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineAlpha = Shader.PropertyToID("_OutlineAlpha");
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");

        public void Outline(bool value, Color color)
        {
            Outlined = value;
            
            if (Outlined)
            {
                _material.SetColor(OutlineColor, color);
                _material.SetFloat(OutlineAlpha, outlineAlpha);
                _material.SetFloat(OutlineWidth, outlineWidth);
            }
            else
            {
                _material.SetFloat(OutlineAlpha, 0f);
                _material.SetFloat(OutlineWidth, 0f);
            }
        }
        
        public void Outline(bool value)
        {
            Outline(value, outlineColor);
        }

        public void SwitchOutline()
        {
            Outline(!Outlined);
        }

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _material = _sprite.material;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (_sprite)
                {
                    if (_material)
                    {
                        Outline(Outlined);
                    }
                }
            }
        }
    }
}