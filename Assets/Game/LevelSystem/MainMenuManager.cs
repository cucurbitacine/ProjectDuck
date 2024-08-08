using System.Collections;
using Game.Core;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.LevelSystem
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float fadeDuration = 1f;
        
        [Header("References")]
        [SerializeField] private Fader fader;
        
        [Header("UI")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueGameButton;
        [SerializeField] private Button quitGameButton;
        
        private void HandleNewGame()
        {
            GameManager.Instance.ResetPlayerData();
            
            StartGame();
        }
        
        private void HandleContinueGame()
        {
            StartGame();
        }
        
        private void HandleQuit()
        {
            QuitGame();
        }

        private void StartGame()
        {
            StartCoroutine(StartingGame());
        }
        
        private void QuitGame()
        {
            StartCoroutine(QuitingGame());
        }
        
        private IEnumerator StartingGame()
        {
            yield return fader?.FadeIn(fadeDuration);

            var startingGame = GameManager.Instance.StartGameAsync();

            yield return new WaitUntil(() => startingGame.isDone);
        }
        
        private IEnumerator QuitingGame()
        {
            yield return fader?.FadeIn(fadeDuration);
            
            GameManager.Instance.Quit();
        }

        private IEnumerator LoadPlayerProfile()
        {
            yield return null;
        }

        [ContextMenu(nameof(ResetPlayerProfile))]
        private void ResetPlayerProfile()
        {
            GameManager.Instance.ResetPlayerData();
        }

        #region MonoBehaviour

        private void OnEnable()
        {
            newGameButton.onClick.AddListener(HandleNewGame);
            continueGameButton.onClick.AddListener(HandleContinueGame);
            quitGameButton.onClick.AddListener(HandleQuit);
        }

        private void OnDisable()
        {
            newGameButton.onClick.RemoveListener(HandleNewGame);
            continueGameButton.onClick.RemoveListener(HandleContinueGame);
            quitGameButton.onClick.RemoveListener(HandleQuit);
        }

        private IEnumerator Start()
        {
            fader?.FadeIn();
            
            newGameButton.interactable = false;
            continueGameButton.interactable = false;
            
            yield return new WaitUntil(() => GameManager.Instance.LoadPlayerDataAsync().IsCompleted);
            
            newGameButton.interactable = true;
            continueGameButton.interactable = !GameManager.Instance.PlayerData.newGame;
            
            yield return fader?.FadeOut(fadeDuration);
        }

        #endregion
    }
}
