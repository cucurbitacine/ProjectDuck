using System.Collections;
using CucuTools.DamageSystem;
using Game.Player;
using Game.Utils;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerDamageEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        [Min(0)]
        [SerializeField] private float delay = 0.2f;
        
        private PlayerController _player;
        private DamageReceiver _damageReceiver;
        private ModelLoader _modelLoader;
        private SpriteRenderer _sprite;
        private Material _material;
        private Coroutine _hitting;
        
        private const string HitEffectKeyword = "HITEFFECT_ON";
        private static readonly int HitEffectBlend = Shader.PropertyToID("_HitEffectBlend");
        private static readonly int HitEffectColor = Shader.PropertyToID("_HitEffectColor");
        
        private void Hit(Color hitColor)
        {
            if (_hitting != null) StopCoroutine(_hitting);
            _hitting = StartCoroutine(Hitting(hitColor));
        }

        private IEnumerator Hitting(Color hitColor)
        {
            var speed = delay > 0 ? 1f / delay : 1f;

            _material?.EnableKeyword(HitEffectKeyword);
            _material?.SetColor(HitEffectColor, hitColor);
            
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
            if (_hitting != null) StopCoroutine(_hitting);
            
            _sprite = model.GetComponent<SpriteRenderer>();
            _material = _sprite.material;
        }

        private void HandleDamage(DamageEvent damageEvent)
        {
            if (damageEvent.damage.amount > 0)
            {
                Hit(damageColor);
            }
            else if (damageEvent.damage.amount < 0)
            {
                Hit(healColor);
            }
        }

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }
        
        private void OnDisable()
        {
            if (_hitting != null) StopCoroutine(_hitting);
        }

        private void Start()
        {
            _damageReceiver = _player.DamageReceiver;
            _modelLoader = _player.ModelLoader;

            _damageReceiver.OnDamageReceived += HandleDamage;
            _modelLoader.OnModelLoaded += HandleModel;
            
            HandleModel(_modelLoader.GetModel());
        }

        private void OnDestroy()
        {
            _damageReceiver.OnDamageReceived -= HandleDamage;
            _modelLoader.OnModelLoaded -= HandleModel;
        }
    }
}
