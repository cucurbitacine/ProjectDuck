using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public sealed class GameManager
    {
        #region Singleton

        public static GameManager Instance { get; private set; }

        static GameManager()
        {
            Instance = new GameManager();
        }

        #endregion
        
        private const string MainMenuSceneName = "MainMenu";
        
        public GameObject Player { get; private set; }
        
        public event Action<GameObject> OnPlayerChanged; 
        
        public AsyncOperation LoadSceneAsync(string sceneName)
        {
            return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
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

        public void SetPlayer(GameObject newPlayer)
        {
            Player = newPlayer;
            
            OnPlayerChanged?.Invoke(newPlayer);
        }

        public void RemovePlayer()
        {
            Player = null;
            
            OnPlayerChanged?.Invoke(null);
        }
    }

    [Serializable]
    public struct LoadingData
    {
        public string sceneName;
    }
}