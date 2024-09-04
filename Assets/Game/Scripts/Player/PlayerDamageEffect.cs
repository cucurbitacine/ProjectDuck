using Game.Scripts.Combat;
using UnityEngine;

namespace Game.Scripts.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerController))]
    public class PlayerDamageEffect : MonoBehaviour
    {
        private PlayerController _player;
        private ModelLoader _modelLoader;
        
        private void HandleModel(GameObject model)
        {
            if (!model.TryGetComponent<DamageEffect>(out var damageEffect))
            {
                damageEffect = model.AddComponent<DamageEffect>();
            }
            
            damageEffect.SetHealth(_player.Health);
        }
        
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }
        
        private void Start()
        {
            _modelLoader = _player.ModelLoader;

            _modelLoader.OnModelLoaded += HandleModel;
            
            HandleModel(_modelLoader.GetModel());
        }

        private void OnDestroy()
        {
            _modelLoader.OnModelLoaded -= HandleModel;
        }
    }
}
