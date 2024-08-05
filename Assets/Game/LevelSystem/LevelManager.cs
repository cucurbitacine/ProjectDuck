using System;
using System.Collections;
using Game.Core;
using UnityEngine;

namespace Game.LevelSystem
{
    public abstract class LevelManager : MonoBehaviour
    {
        #region Static

        private static LevelManager _current;
        
        public static LevelManager Instance
        {
            get => _current;
            private set
            {
                _current = value;
                
                OnLevelChanged?.Invoke(_current);
            }
        }

        public static event Action<LevelManager> OnLevelChanged;
        
        #endregion

        public enum LevelResult
        {
            Won,
            Failed,
        }
        
        [SerializeField] private float fadeTime = 1f;
        [SerializeField] private Fader fader;

        private bool _goingToMainMenu;

        public event Action<LevelResult> OnLevelEnded;
        
        [ContextMenu(nameof(Win))]
        public void Win()
        {
            //
            
            OnLevelEnded?.Invoke(LevelResult.Won);
        }

        [ContextMenu(nameof(Fail))]
        public void Fail()
        {
            //

            OnLevelEnded?.Invoke(LevelResult.Failed);
        }
        
        [ContextMenu(nameof(GoToMainMenu))]
        public void GoToMainMenu()
        {
            if (_goingToMainMenu) return;
            
            StartCoroutine(GoingToMainMenu());
        }
        
        protected virtual IEnumerator PrepareLevel()
        {
            yield return null;
        }
        
        protected virtual void OnStartLevel()
        {
        }
        
        protected virtual void OnStopLevel()
        {
        }
        
        protected virtual IEnumerator DisposeLevel()
        {
            yield return null;
        }
        
        private IEnumerator GoingToMainMenu()
        {
            _goingToMainMenu = true;

            OnStopLevel();
            
            yield return fader?.FadeIn(fadeTime);

            yield return DisposeLevel();
            
            yield return GameManager.Instance.MainMenuAsync();
        }
        
        private IEnumerator Start()
        {
            Instance = this;

            fader?.FadeIn();
            
            yield return PrepareLevel();
            
            yield return fader?.FadeOut(fadeTime);
            
            OnStartLevel();
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}