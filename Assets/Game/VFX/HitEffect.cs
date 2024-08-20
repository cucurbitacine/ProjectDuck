using System.Collections;
using UnityEngine;

namespace Game.VFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class HitEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float delay = 0.2f;
        
        private SpriteRenderer _sprite;
        private Material _material;
        private Coroutine _hitting;
        
        //private const string HitEffectKeyword = "HITEFFECT_ON";
        private static readonly int HitEffectBlend = Shader.PropertyToID("_HitEffectBlend");
        private static readonly int HitEffectColor = Shader.PropertyToID("_HitEffectColor");
        
        public void Hit(Color hitColor)
        {
            if (_hitting != null) StopCoroutine(_hitting);
            _hitting = StartCoroutine(Hitting(hitColor));
        }

        private IEnumerator Hitting(Color hitColor)
        {
            var speed = delay > 0 ? 1f / delay : 1f;

            //_material.EnableKeyword(HitEffectKeyword);
            _material.SetColor(HitEffectColor, hitColor);
            
            var blend = 1f;
            while (blend > 0f)
            {
                _material.SetFloat(HitEffectBlend, blend);
                
                blend -= speed * Time.deltaTime;
                yield return null;
            }
            
            _material.SetFloat(HitEffectBlend, 0f);
            //_material.DisableKeyword(HitEffectKeyword);
        }

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _material = _sprite.material;
        }
        
        private void OnDisable()
        {
            if (_hitting != null) StopCoroutine(_hitting);
            
            _material.SetFloat(HitEffectBlend, 0f);
            //_material.DisableKeyword(HitEffectKeyword);
        }
    }
}