using System;
using System.Collections;
using Game.Core;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.LevelSystem
{
    public class CreditsSceneManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float fadeOutTime = 2f;
        [SerializeField] private float fadeInTime = 4f;
        
        [Header("References")]
        [SerializeField] private Fader fader;
        
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
            
            yield return fader?.FadeOut(fadeOutTime);
            
            mainMenuButton.gameObject.SetActive(true);
        }
    }
}
