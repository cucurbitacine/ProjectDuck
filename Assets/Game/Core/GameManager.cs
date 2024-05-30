using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class GameManager
    {
        #region Singleton

        public static GameManager Instance { get; private set; }

        static GameManager()
        {
            Instance = new GameManager();
        }

        #endregion
        
        private const string LoadingSceneName = "Loading";
        
        public const string MainMenuSceneName = "MainMenu";
        
        public LoadingData LoadingData { get; private set; }

        public AsyncOperation LoadSceneWithLoadingScreenAsync(string sceneName)
        {
            LoadingData = new LoadingData()
            {
                sceneName = sceneName,
            };
            
            return SceneManager.LoadSceneAsync(LoadingSceneName, LoadSceneMode.Single);
        }
        
        public AsyncOperation MainMenuAsync()
        {
            return SceneManager.LoadSceneAsync(MainMenuSceneName, LoadSceneMode.Single);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    [Serializable]
    public struct LoadingData
    {
        public string sceneName;
    }
}