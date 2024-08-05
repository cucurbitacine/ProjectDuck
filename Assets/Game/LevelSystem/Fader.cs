using System.Collections;
using UnityEngine;

namespace Game.LevelSystem
{
    [DisallowMultipleComponent]
    public class Fader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;

        private Coroutine _fading;

        private const float MinFade = 0f;
        private const float MaxFade = 1f;
        private static float FadeLength => MaxFade - MinFade;

        public float FadeValue
        {
            get => canvasGroup ? canvasGroup.alpha : 0f;
            set
            {
                if (canvasGroup)
                {
                    canvasGroup.alpha = Mathf.Clamp(value, MinFade, MaxFade);
                }
            }
        }

        public void FadeIn()
        {
            FadeIn(0f);
        }
        
        public void FadeOut()
        {
            FadeOut(0f);
        }
        
        public Coroutine FadeIn(float duration)
        {
            return Fade(MinFade, MaxFade, duration);
        }
        
        public Coroutine FadeOut(float duration)
        {
            return Fade(MaxFade, MinFade, duration);
        }
        
        public Coroutine Fade(float begin, float end, float duration)
        {
            if (_fading != null) StopCoroutine(_fading);
            _fading = StartCoroutine(Fading(begin, end, duration));
            
            return _fading;
        }

        private IEnumerator Fading(float begin, float end, float duration)
        {
            begin = Mathf.Clamp(begin, MinFade, MaxFade);
            end = Mathf.Clamp(end, MinFade, MaxFade);
            
            var fadeSpeed = duration > 0f ? (FadeLength / duration) : 0f;
            var fadeDuration = fadeSpeed > 0f ? Mathf.Abs(end - begin) / fadeSpeed : 0f;

            var fadeTime = 0f;
            FadeValue = begin;
            while (fadeDuration > 0f && fadeTime < fadeDuration)
            {
                var t = fadeTime / fadeDuration;
                t = Mathf.Clamp(t, MinFade, MaxFade);
                t = Mathf.SmoothStep(MinFade, MaxFade, t);
                FadeValue = Mathf.Lerp(begin, end, t); 
                
                yield return null;
                fadeTime += Time.deltaTime;
            }
            FadeValue = end;
        }
    }
}