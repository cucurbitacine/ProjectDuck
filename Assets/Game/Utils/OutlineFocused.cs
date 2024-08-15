using Game.InteractionSystem;
using UnityEngine;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    public class OutlineFocused : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteTarget;
        [SerializeField] private GameObject focusSource;

        private Material _material;
        private IFocused _focused;

        private const string OutlineEffectKeyword = "OUTBASE_ON";
        
        private void HandleFocus(bool focused)
        {
            if (focused)
            {
                _material.EnableKeyword(OutlineEffectKeyword);
            }
            else
            {
                _material.DisableKeyword(OutlineEffectKeyword);
            }
        }
        
        private void Awake()
        {
            _focused = focusSource.GetComponent<IFocused>();
        }

        private void OnEnable()
        {
            _focused.OnFocusChanged += HandleFocus;
        }

        private void OnDisable()
        {
            _focused.OnFocusChanged -= HandleFocus;
        }

        private void Start()
        {
            _material = spriteTarget.material;
            
            HandleFocus(_focused.Focused);
        }
    }
}