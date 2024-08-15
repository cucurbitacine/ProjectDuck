using System;
using System.Collections;
using Game.Core;
using Game.Player;
using Game.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Game.LevelSystem
{
    [DisallowMultipleComponent]
    public class LevelManager : MonoBehaviour
    {
        #region Static
        
        public static PlayerController Player { get; private set; }
        
        public static event Action<PlayerController> OnPlayerChanged; 
        
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
        
        [SerializeField] private bool busy = false;
        [SerializeField] private int levelNumber = 0;
        
        [Header("Settings")]
        [SerializeField] private float fadeOutTime = 2f;
        [SerializeField] private float fadeInTime = 4f;

        [Header("Events")]
        [SerializeField] private UnityEvent onLevelStarted = new UnityEvent();
        
        [Header("References")]
        [SerializeField] private Fader fader;

        public bool Busy
        {
            get => busy;
            private set => busy = value;
        }

        public event Action<LevelResult> OnLevelEnded;
        
        [ContextMenu(nameof(CompleteLevel))]
        public void CompleteLevel()
        {
            if (busy) return;
            
            //
            
            OnLevelEnded?.Invoke(LevelResult.Won);
            
            GoToNextLevel();
        }

        [ContextMenu(nameof(FailLevel))]
        public void FailLevel()
        {
            if (Busy) return;
            
            //

            OnLevelEnded?.Invoke(LevelResult.Failed);

            RestartLevel();
        }
        
        [ContextMenu(nameof(GoToMainMenu))]
        public void GoToMainMenu()
        {
            if (Busy) return;
            
            StartCoroutine(GoingToMainMenu());
        }

        [ContextMenu(nameof(GoToNextLevel))]
        public void GoToNextLevel()
        {
            if (Busy) return;
            
            StartCoroutine(GoingToNextLevel());
        }

        [ContextMenu(nameof(RestartLevel))]
        public void RestartLevel()
        {
            if (Busy) return;
            
            StartCoroutine(RestartingLevel());
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
            yield return new WaitUntil(() => Player);
            
            Player.Pause(true);

            Player.Health.OnDied += HandlePlayerDeath;

            var playerData = GameManager.Instance.GetPlayerData();
            playerData.levelNumber = levelNumber;
            
            yield return new WaitUntil(() => GameManager.Instance.SavePlayerDataAsync().IsCompleted);
        }

        private IEnumerator ShutdownLevel()
        {
            Busy = true;
            
            Player.Health.OnDied -= HandlePlayerDeath;
            
            OnStopLevel();
            
            yield return fader?.FadeIn(fadeInTime);

            Player.Pause(true);
            
            yield return DisposeLevel();
        }
        
        private IEnumerator GoingToMainMenu()
        {
            yield return ShutdownLevel();
            
            yield return GameManager.Instance.LoadMainMenuAsync();
        }
        
        private IEnumerator GoingToNextLevel()
        {
            yield return ShutdownLevel();
            
            yield return GameManager.Instance.LoadNextLevelAsync();
        }

        private IEnumerator RestartingLevel()
        {
            yield return ShutdownLevel();
            
            yield return GameManager.Instance.StartGameAsync();
        }
        
        private void HandlePlayerDeath()
        {
            FailLevel();
        }
        
        private void Awake()
        {
            Busy = true;
        }

        private IEnumerator Start()
        {
            fader?.FadeIn();

            yield return PreparePlayer();
            
            yield return PrepareLevel();
            
            yield return fader?.FadeOut(fadeOutTime);
            
            Player.Pause(false);
            
            Busy = false;
            
            OnStartLevel();
            
            onLevelStarted.Invoke();
        }

        private void Update()
        {
            if (Application.isEditor) // TODO DEVELOPMENT PURPOSE ONLY 
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    CompleteLevel();
                }
                
                if (Input.GetKeyDown(KeyCode.L))
                {
                    FailLevel();
                }
            }
        }
    }
}