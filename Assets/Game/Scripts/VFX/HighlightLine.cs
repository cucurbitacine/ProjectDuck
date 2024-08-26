using System.Collections;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.VFX
{
    [RequireComponent(typeof(LineRenderer))]
    public class HighlightLine : MonoBehaviour, IPaused
    {
        [field: SerializeField] public bool Paused { get; private set; }
        
        [SerializeField] private Color highlightColor = Color.red;
        
        [Space]
        [Min(0f)] [SerializeField] private float duration = 0.2f;
        [SerializeField] private bool onlyOnce = false;
        
        private LineRenderer _line;
        private Color _baseColor;

        private Coroutine _highliging;
        
        public void Highlight(bool isOn)
        {
            if (Paused) return;
            
            _line.startColor = isOn ? highlightColor : _baseColor;
            _line.endColor = isOn ? highlightColor : _baseColor;

            if (_highliging != null) StopCoroutine(_highliging);
            
            if (isOn && !onlyOnce)
            {
                _highliging = StartCoroutine(Highlighting());
            }
        }

        private IEnumerator Highlighting()
        {
            yield return new WaitForSeconds(duration);

            Highlight(false);
        }
        
        private void Awake()
        {
            _line = GetComponent<LineRenderer>();

            _baseColor = _line.startColor;
        }
        
        public void Pause(bool value)
        {
            Paused = value;
        }
    }
}