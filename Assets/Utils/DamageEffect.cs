using System.Collections;
using CucuTools.DamageSystem;
using Game.Utils;
using UnityEngine;

namespace Utils
{
    public class DamageEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Color color = Color.red;
        [Min(0)]
        [SerializeField] private float delay = 0.2f;
        
        [Header("References")]
        [SerializeField] private DamageReceiver damageReceiver;
        [SerializeField] private ModelLoader modelLoader;

        private SpriteRenderer _sprite;
        private Material _material;
        private Coroutine _hitting;
        
        private const string HitEffectKeyword = "HITEFFECT_ON";
        private static readonly int HitEffectBlend = Shader.PropertyToID("_HitEffectBlend");
        private static readonly int HitEffectColor = Shader.PropertyToID("_HitEffectColor");
        
        private void Hit()
        {
            if (_hitting != null) StopCoroutine(_hitting);
            _hitting = StartCoroutine(Hitting());
        }

        private IEnumerator Hitting()
        {
            var speed = delay > 0 ? 1f / delay : 1f;

            _material?.EnableKeyword(HitEffectKeyword);
            _material?.SetColor(HitEffectColor, color);
            
            var blend = 1f;
            while (blend > 0f)
            {
                _material?.SetFloat(HitEffectBlend, blend);
                
                blend -= speed * Time.deltaTime;
                yield return null;
            }
            
            _material?.SetFloat(HitEffectBlend, 0f);
            _material?.DisableKeyword(HitEffectKeyword);
        }
        
        private void HandleModel(GameObject model)
        {
            _sprite = model.GetComponent<SpriteRenderer>();
            _material = _sprite.material;
        }
        
        private void HandleDamage(DamageEvent damageEvent)
        {
            if (damageEvent.damage.amount > 0)
            {
                Hit();
            }
        }
        
        private void OnEnable()
        {
            modelLoader.OnModelLoaded += HandleModel;
            damageReceiver.OnDamageReceived += HandleDamage;
        }

        private void OnDisable()
        {
            modelLoader.OnModelLoaded -= HandleModel;
            damageReceiver.OnDamageReceived -= HandleDamage;

            if (_hitting != null) StopCoroutine(_hitting);
        }

        private void Start()
        {
            HandleModel(modelLoader.GetModel());
        }
    }
}
