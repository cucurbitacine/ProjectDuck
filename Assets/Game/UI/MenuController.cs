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
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private UIInput uiInput;
        
        [Header("UI")]
        [SerializeField] private GameObject menuCanvas;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        public void Pause(bool value)
        {
            if (levelManager.Busy) return;
            
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
            levelManager.RestartLevel();
        }
        
        private void HandleMainMenuButton()
        {
            levelManager.GoToMainMenu();
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
            levelManager = FindObjectOfType<LevelManager>();
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
