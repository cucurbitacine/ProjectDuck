using System;
using System.Collections;
using Game.Core;
using Game.Player;
using UnityEngine;

namespace Game.LevelSystem
{
    [DisallowMultipleComponent]
    public abstract class LevelManager : MonoBehaviour
    {
        #region Static

        private static LevelManager _current;
        
        public static LevelManager Current
        {
            get => _current;
            private set
            {
                _current = value;
                
                OnLevelChanged?.Invoke(_current);
            }
        }
        
        public static PlayerController Player { get; private set; }
        
        public static event Action<PlayerController> OnPlayerChanged; 
        public static event Action<LevelManager> OnLevelChanged;
        
        public static void SetPlayer(PlayerController newPlayer)
        {
            Player = newPlayer;
            
            OnPlayerChanged?.Invoke(newPlayer);
        }

        public static void RemovePlayer()
        {
            Player = null;
            
            OnPlayerChanged?.Invoke(null);
        }
        
        #endregion

        public enum LevelResult
        {
            Won,
            Failed,
        }
        
        [SerializeField] private float fadeTime = 1f;
        [SerializeField] private Fader fader;

        public event Action<LevelResult> OnLevelEnded;
        
        [ContextMenu(nameof(Win))]
        public void Win()
        {
            //
            
            OnLevelEnded?.Invoke(LevelResult.Won);
            
            GoToNextLevel();
        }

        [ContextMenu(nameof(Fail))]
        public void Fail()
        {
            //

            OnLevelEnded?.Invoke(LevelResult.Failed);

            GoToNextLevel();
        }
        
        [ContextMenu(nameof(GoToMainMenu))]
        public void GoToMainMenu()
        {
            StartCoroutine(GoingToMainMenu());
        }

        [ContextMenu(nameof(GoToNextLevel))]
        public void GoToNextLevel()
        {
            StartCoroutine(GoingToNextLevel());
        }
        
        #region Virtual API

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

        #endregion

        private IEnumerator PreparePlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.SavePlayerDataAsync().IsCompleted);
        }
        
        private IEnumerator ShutdownLevel()
        {
            OnStopLevel();
            
            yield return fader?.FadeIn(fadeTime);

            yield return DisposeLevel();
        }
        
        private IEnumerator GoingToMainMenu()
        {
            yield return ShutdownLevel();
            
            yield return GameManager.Instance.MainMenuAsync();
        }
        
        private IEnumerator GoingToNextLevel()
        {
            yield return ShutdownLevel();
            
            yield return GameManager.Instance.LoadNextLevelAsync();
        }
        
        private IEnumerator Start()
        {
            Current = this;

            fader?.FadeIn();

            yield return PreparePlayer();
            
            yield return PrepareLevel();
            
            yield return fader?.FadeOut(fadeTime);
            
            OnStartLevel();
        }

        private void OnDestroy()
        {
            Current = null;
        }
    }
}