using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public class WorldSpaceCanvas : MonoBehaviour
    {
        private Canvas _canvas;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();

            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = Camera.main;
        }
        
        private void OnValidate()
        {
            _canvas = GetComponent<Canvas>();

            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = Camera.main;
        }
    }
}