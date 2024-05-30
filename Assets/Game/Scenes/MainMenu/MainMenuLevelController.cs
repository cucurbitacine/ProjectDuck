using System.Collections;
using Game.Core;
using Game.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scenes.MainMenu
{
    public class MainMenuLevelController : LevelController
    {
        [Header("Settings")]
        [Min(0f)]
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private string introSceneName = "LevelIntro";
        
        [Header("References")]
        [SerializeField] private Fader fader;
        
        [Header("UI")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        private void HandlePlayButton()
        {
            StartCoroutine(Loading(introSceneName));
        }

        private IEnumerator Loading(string sceneName)
        {
            if (playButton) playButton.interactable = false;
            if (quitButton) quitButton.interactable = false;
            
            if (fader)
            {
                yield return fader.FadeIn(fadeDuration);
            }

            var loading = Game.LoadSceneWithLoadingScreenAsync(sceneName);

            yield return new WaitUntil(() => loading.isDone);
        }
        
        private void HandleQuitButton()
        {
            StartCoroutine(Quiting());
        }
        
        private IEnumerator Quiting()
        {
            if (playButton) playButton.interactable = false;
            if (quitButton) quitButton.interactable = false;
            
            if (fader)
            {
                yield return fader.FadeIn(fadeDuration);
            }
            
            Game.Quit();
        }
        
        private void OnEnable()
        {
            if (playButton)
            {
                playButton.onClick.AddListener(HandlePlayButton);
            }
            
            if (quitButton)
            {
                quitButton.onClick.AddListener(HandleQuitButton);
            }
        }

        private void OnDisable()
        {
            if (playButton)
            {
                playButton.onClick.RemoveListener(HandlePlayButton);
            }
            
            if (quitButton)
            {
                quitButton.onClick.RemoveListener(HandleQuitButton);
            }
        }

        private IEnumerator Start()
        {
            if (fader)
            {
                if (playButton) playButton.interactable = false;
                if (quitButton) quitButton.interactable = false;
                
                yield return fader.FadeOut(fadeDuration);
            }
            
            if (playButton) playButton.interactable = true;
            if (quitButton) quitButton.interactable = true;
        }
    }
}
