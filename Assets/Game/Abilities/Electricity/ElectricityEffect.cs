using UnityEngine;

namespace Game.Abilities.Electricity
{
    [RequireComponent(typeof(IElectricityStorage))]
    public class ElectricityEffect : MonoBehaviour
    {
        [SerializeField] [Min(0)] private int emissionRateOverTime = 1000;
        [SerializeField] private Color electricityOutlineColor = Color.cyan;
        
        [Header("References")]
        [SerializeField] private SpriteRenderer surface;
        [SerializeField] private GameObject effectPrefab;
        
        private IElectricityStorage _storage;
        private ParticleSystem _effect;
        
        public void SetSpriteRendererSurface(SpriteRenderer spriteRenderer)
        {
            surface = spriteRenderer;
        }
        
        public void ShowEffect()
        {
            var effect = GetEffect();

            if (surface)
            {
                var emission = effect.emission;
                emission.rateOverTime = emissionRateOverTime;
                
                var shape = effect.shape;
                shape.shapeType = ParticleSystemShapeType.SpriteRenderer;
                shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
                shape.spriteRenderer = surface;
                shape.texture = surface.sprite.texture;
            }
            
            effect.Play();
        }

        public void HideEffect()
        {
            var effect = GetEffect();
            
            effect.Stop();
        }

        private ParticleSystem GetEffect()
        {
            if (_effect) return _effect;

            _effect = Instantiate(effectPrefab, transform).GetComponent<ParticleSystem>();

            return _effect;
        }
        
        private void HandleCharge(int value)
        {
            if (value > 0)
            {
                ShowEffect();
            }
            else
            {
                HideEffect();
            }
        }
        
        private void Awake()
        {
            _storage = GetComponent<IElectricityStorage>();

            if (surface == null) surface = GetComponentInChildren<SpriteRenderer>();
            
            SetSpriteRendererSurface(surface);
        }

        private void OnValidate()
        {
            if (surface == null) surface = GetComponentInChildren<SpriteRenderer>();
            
            if (Application.isPlaying)
            {
                if (_effect && _effect.isPlaying)
                {
                    var emission = _effect.emission;
                    emission.rateOverTime = emissionRateOverTime;
                }
            }
        }

        private void OnEnable()
        {
            _storage.OnChargeChanged += HandleCharge;
            
            HandleCharge(_storage.ElectricityCharge);
        }

        private void OnDisable()
        {
            _storage.OnChargeChanged -= HandleCharge;
        }

        private void Start()
        {
            //HandleCharge(_storage.ElectricityCharge);
        }
    }
}