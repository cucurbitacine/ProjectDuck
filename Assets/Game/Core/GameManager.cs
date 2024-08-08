using System;
using System.Threading.Tasks;
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

        private static PlayerData CreateNewPlayerData()
        {
            return new PlayerData()
            {
                playerName = "DefaultPlayerName",
            };
        }
        
        private static int GetSceneBuildIndexByLevelNumber(int currentLevel)
        {
            var buildIndex = -1;

            if (currentLevel < 0)
            {
                buildIndex = SceneBuildIndex_FirstLevel;
            }
            else
            {
                buildIndex = currentLevel + SceneBuildIndex_FirstLevel;
            }

            if (buildIndex < 0 || SceneManager.sceneCountInBuildSettings <= buildIndex)
            {
                buildIndex = SceneBuildIndex_MainMenu;
            }

            return buildIndex;
        }
        
        #endregion
        
        private const int SceneBuildIndex_MainMenu = 0;
        private const int SceneBuildIndex_FirstLevel = 1;
        
        private const string KeyName_PlayerData = nameof(PlayerData);

        public PlayerData PlayerData { get; private set; }
        
        public async Task LoadPlayerDataAsync()
        {
            try
            {
                var jsonProfile = PlayerPrefs.GetString(KeyName_PlayerData);
                PlayerData = JsonUtility.FromJson<PlayerData>(jsonProfile) ?? CreateNewPlayerData();
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message} - {e.StackTrace}");
                
                PlayerData = CreateNewPlayerData();
            }
            
            await Task.CompletedTask;
            
            Debug.Log($"[LOADED PLAYER] {PlayerData}");
        }
        
        public async Task SavePlayerDataAsync()
        {
            if (PlayerData == null)
            {
                PlayerData = CreateNewPlayerData();
            }
            
            var jsonProfile = JsonUtility.ToJson(PlayerData);
            
            PlayerPrefs.SetString(KeyName_PlayerData, jsonProfile);
            
            await Task.CompletedTask;
            
            Debug.Log($"[SAVED PLAYER] {PlayerData}");
        }

        public async void ResetPlayerData()
        {
            PlayerData = CreateNewPlayerData();

            Debug.Log($"[RESET PLAYER] {PlayerData}");
            
            await SavePlayerDataAsync();
        }

        public AsyncOperation StartGameAsync()
        {
            if (PlayerData.levelNumber < 0)
            {
                PlayerData.levelNumber = 0;
            }
            
            var buildIndex = GetSceneBuildIndexByLevelNumber(PlayerData.levelNumber);
            
            return LoadSceneAsync(buildIndex);
        }
        
        public AsyncOperation LoadNextLevelAsync()
        {
            if (PlayerData.levelNumber < 0)
            {
                PlayerData.levelNumber = 0;
            }
            else
            {
                PlayerData.levelNumber++;
            }
            
            var buildIndex = GetSceneBuildIndexByLevelNumber(PlayerData.levelNumber);
            
            return LoadSceneAsync(buildIndex);
        }
        
        public AsyncOperation MainMenuAsync()
        {
            return LoadSceneAsync(SceneBuildIndex_MainMenu);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
        private AsyncOperation LoadSceneAsync(int sceneBuildIndex)
        {
            Debug.Log($"[LOADING SCENE] \"{sceneBuildIndex}\" {PlayerData}");
            
            return SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);
        }
    }
}