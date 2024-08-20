using Game.InteractionSystem;
using UnityEngine;

namespace Game.VFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IFocused))]
    public class FocusEffect : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OutlineEffect outline;
        
        private IFocused _focused;
        
        private void HandleFocus(bool focused)
        {
            outline.Outline(focused);
        }
        
        private void Awake()
        {
            _focused = GetComponent<IFocused>();
            
            if (outline == null) outline = GetComponentInChildren<OutlineEffect>();
        }

        private void OnValidate()
        {
            if (outline == null) outline = GetComponentInChildren<OutlineEffect>();
        }

        private void OnEnable()
        {
            _focused.OnFocusChanged += HandleFocus;
        }

        private void OnDisable()
        {
            _focused.OnFocusChanged -= HandleFocus;
            
            HandleFocus(false);
        }

        private void Start()
        {
            HandleFocus(_focused.Focused);
        }
    }
}