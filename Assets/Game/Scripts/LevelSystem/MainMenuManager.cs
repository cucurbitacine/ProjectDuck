using System.Collections;
using Cinemachine;
using Game.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.LevelSystem
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] [Min(0f)] private float fadeDuration = 2f;
        [SerializeField] [Min(0f)] private float camBlend = 3f;
        
        [Header("References")]
        [SerializeField] private ScreenFader fader;
        [SerializeField] private CinemachineVirtualCameraBase mainCamera;
        
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
            
            newGameButton.gameObject.SetActive(false);
            continueGameButton.gameObject.SetActive(false);
            quitGameButton.gameObject.SetActive(false);

            var gettingPlayerData = GameManager.Instance.GetPlayerDataAsync();
            yield return new WaitUntil(() => gettingPlayerData.IsCompleted);
            var playerData = gettingPlayerData.Result;
            
            yield return fader?.FadeOut(fadeDuration);

            if (mainCamera)
            {
                var brain = VCam.GetBrain();
                brain.m_DefaultBlend =
                    new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, camBlend);
                VCam.SetActive(mainCamera);
                yield return new WaitForSeconds(camBlend);
            }
            
            newGameButton.gameObject.SetActive(true);
            continueGameButton.gameObject.SetActive(0 <= playerData.levelNumber && playerData.levelNumber < GameManager.TotalNumberLevels);
            quitGameButton.gameObject.SetActive(true);
        }

        #endregion
    }
}
