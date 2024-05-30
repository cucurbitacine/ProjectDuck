using System.Collections;
using Game.Core;
using Game.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scenes.Levels
{
    public class GameLevelController : LevelController
    {
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float fadeDuration = 1f;
        
        [Header("References")]
        [SerializeField] private Fader fader;
        
        [Header("UI")]
        [SerializeField] private Button returnButton;

        private void HandleReturnButton()
        {
            StartCoroutine(Returning());
        }

        private IEnumerator Returning()
        {
            if (returnButton) returnButton.interactable = false;
            
            if (fader)
            {
                yield return fader.FadeIn(fadeDuration);
            }
            
            var loading = Game.LoadSceneWithLoadingScreenAsync(GameManager.MainMenuSceneName);

            yield return new WaitUntil(() => loading.isDone);
        }
        
        private void OnEnable()
        {
            if (returnButton)
            {
                returnButton.onClick.AddListener(HandleReturnButton);
            }
        }
        
        private void OnDisable()
        {
            if (returnButton)
            {
                returnButton.onClick.RemoveListener(HandleReturnButton);
            }
        }

        private IEnumerator Start()
        {
            if (fader)
            {
                if (returnButton) returnButton.interactable = false;
                
                yield return fader.FadeOut(fadeDuration);
            }
            
            if (returnButton) returnButton.interactable = true;
        }
    }
}
