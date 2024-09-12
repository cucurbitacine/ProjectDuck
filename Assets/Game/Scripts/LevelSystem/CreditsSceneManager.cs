using System.Collections;
using Game.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.LevelSystem
{
    public class CreditsSceneManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float fadeOutTime = 2f;
        [SerializeField] private float fadeInTime = 4f;
        
        [Header("References")]
        [SerializeField] private ScreenFader fader;
        
        [Header("UI")]
        [SerializeField] private Button mainMenuButton;

        private void HandleMainMenuButton()
        {
            StartCoroutine(GoingToMainMenu());
        }

        private IEnumerator GoingToMainMenu()
        {
            mainMenuButton.gameObject.SetActive(false);
            
            yield return fader?.FadeIn(fadeInTime);
            
            yield return GameManager.Instance.LoadMainMenuAsync();
        }
        
        private void OnEnable()
        {
            mainMenuButton.onClick.AddListener(HandleMainMenuButton);
        }
        
        private void OnDisable()
        {
            mainMenuButton.onClick.RemoveListener(HandleMainMenuButton);
        }

        private IEnumerator Start()
        {
            mainMenuButton.gameObject.SetActive(false);
            
            fader?.FadeIn();

            var gettingPlayerData = GameManager.Instance.GetPlayerDataAsync();
            yield return new WaitUntil(() => gettingPlayerData.IsCompleted);
            var savingPlayerData = GameManager.Instance.SavePlayerDataAsync(gettingPlayerData.Result);
            yield return new WaitUntil(() => savingPlayerData.IsCompleted);
            
            yield return fader?.FadeOut(fadeOutTime);
            
            mainMenuButton.gameObject.SetActive(true);
        }
    }
}
