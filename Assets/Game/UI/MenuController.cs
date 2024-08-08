using System;
using Game.LevelSystem;
using Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MenuController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool paused = false;
        
        [Header("References")]
        [SerializeField] private UIInput uiInput;
        
        [Header("UI")]
        [SerializeField] private GameObject menuCanvas;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private LevelManager _levelManager;
        
        public void Pause(bool value)
        {
            if (_levelManager.Busy) return;
            
            paused = value;
            
            menuCanvas.SetActive(value);

            Time.timeScale = value ? 0.001f : 1f;
            
            LevelManager.Player.Pause(value);
        }
        
        private void HandleContinueButton()
        {
            Pause(false);
        }
        
        private void HandleRestartButton()
        {
            _levelManager.RestartLevel();
        }
        
        private void HandleMainMenuButton()
        {
            _levelManager.GoToMainMenu();
        }

        private void HandleCancel(bool value)
        {
            if (value)
            {
                Pause(!paused);
            }
        }
        
        private void Awake()
        {
            _levelManager = FindObjectOfType<LevelManager>();
        }

        private void OnEnable()
        {
            continueButton.onClick.AddListener(HandleContinueButton);
            restartButton.onClick.AddListener(HandleRestartButton);
            mainMenuButton.onClick.AddListener(HandleMainMenuButton);
            
            uiInput.CancelEvent += HandleCancel;
        }
        
        private void OnDisable()
        {
            continueButton.onClick.RemoveListener(HandleContinueButton);
            restartButton.onClick.RemoveListener(HandleRestartButton);
            mainMenuButton.onClick.RemoveListener(HandleMainMenuButton);
            
            uiInput.CancelEvent -= HandleCancel;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
        
        private void Start()
        {
            menuCanvas.SetActive(paused);
        }
    }
}
