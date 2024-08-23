using System.Collections;
using Game.Core;
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
        [SerializeField] private ScreenFader fader;
        
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
            newGameButton.gameObject.SetActive(false);
            continueGameButton.gameObject.SetActive(false);
            quitGameButton.gameObject.SetActive(false);
            
            StartCoroutine(StartingGame());
        }
        
        private void QuitGame()
        {
            newGameButton.gameObject.SetActive(false);
            continueGameButton.gameObject.SetActive(false);
            quitGameButton.gameObject.SetActive(false);
            
            StartCoroutine(QuitingGame());
        }
        
        private IEnumerator StartingGame()
        {
            yield return fader?.FadeIn(fadeDuration);

            var gettingPlayerData = GameManager.Instance.GetPlayerDataAsync();
            yield return new WaitUntil(() => gettingPlayerData.IsCompleted);
            var playerData = gettingPlayerData.Result;
            
            var startingGame = GameManager.Instance.StartGameAsync(playerData);

            yield return new WaitUntil(() => startingGame.isDone);
        }
        
        private IEnumerator QuitingGame()
        {
            yield return fader?.FadeIn(fadeDuration);
            
            GameManager.Instance.Quit();
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
            continueGameButton.gameObject.SetActive(false);

            var gettingPlayerData = GameManager.Instance.GetPlayerDataAsync();
            yield return new WaitUntil(() => gettingPlayerData.IsCompleted);
            var playerData = gettingPlayerData.Result;
            
            newGameButton.interactable = true;
            continueGameButton.gameObject.SetActive(!playerData.newGame);
            
            yield return fader?.FadeOut(fadeDuration);
        }

        #endregion
    }
}
