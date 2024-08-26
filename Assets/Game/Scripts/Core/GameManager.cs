using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core
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
            Debug.LogWarning($"[CREATE PLAYER] NEW");
            
            return new PlayerData()
            {
                playerId = Random.Range(100,1000),
                attemptNumber = Random.Range(10,100),
            };
        }
        
        private static bool TryGetSceneBuildIndexByLevelNumber(int levelNumber, out int buildIndex)
        {
            if (0 <= levelNumber && levelNumber < TotalNumberLevels)
            {
                buildIndex = SceneBuildIndex_FirstLevel + levelNumber;
                return true;
            }

            if (levelNumber < 0)
            {
                buildIndex = SceneBuildIndex_FirstLevel;
                return true;
            }

            if (TotalNumberLevels <= levelNumber)
            {
                buildIndex = SceneBuildIndex_CreditsScene;
                return true;
            }
            
            buildIndex = SceneBuildIndex_MainMenu;
            return false;
        }
        
        #endregion
        
        private const int SceneBuildIndex_MainMenu = 0;
        private const int SceneBuildIndex_CreditsScene = SceneBuildIndex_FirstLevel + TotalNumberLevels;
        private const int SceneBuildIndex_FirstLevel = 1;
        private const int TotalNumberLevels = 2;

        #region PlayerData Logic

        private const string KeyName_PlayerData = nameof(PlayerData);

        private PlayerData PlayerData { get; set; }
        
        public async Task<PlayerData> GetPlayerDataAsync()
        {
            if (PlayerData == null)
            {
                Debug.LogWarning($"[GET PLAYER] NULL");
                
                PlayerData = await LoadPlayerDataAsync();
            }

            Debug.Log($"[GET PLAYER] \"{PlayerData}\"");
            
            return PlayerData;
        }
        
        public async Task<PlayerData> LoadPlayerDataAsync()
        {
            PlayerData playerData;
            
            try
            {
                await Task.CompletedTask;
                
                var jsonData = PlayerPrefs.GetString(KeyName_PlayerData);

                playerData = JsonUtility.FromJson<PlayerData>(jsonData) ?? CreateNewPlayerData();
                
                Debug.Log($"[LOADED PLAYER] \"{playerData}\" from \"{jsonData}\"");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Message: \"{e.Message}\"]\n[StackTrace: \"{e.StackTrace}\"]");

                playerData = CreateNewPlayerData();
                
                Debug.Log($"[LOADED PLAYER] \"{playerData}\"");
            }
            
            return playerData;
        }
        
        public async Task SavePlayerDataAsync(PlayerData playerData)
        {
            var jsonData = JsonUtility.ToJson(playerData);
            
            PlayerPrefs.SetString(KeyName_PlayerData, jsonData);
            
            Debug.Log($"[SAVED PLAYER] \"{playerData}\" as \"{jsonData}\"");
            
            await Task.CompletedTask;
        }

        public void ResetPlayerData()
        {
            PlayerPrefs.DeleteKey(KeyName_PlayerData);
            
            PlayerData = null;

            Debug.Log($"[RESET PLAYER]");
        }

        #endregion

        public AsyncOperation StartGameAsync(PlayerData playerData)
        {
            if (playerData.levelNumber < 0)
            {
                playerData.levelNumber = 0;
            }

            var sceneBuildIndex = TryGetSceneBuildIndexByLevelNumber(playerData.levelNumber, out var buildIndex)
                ? buildIndex
                : SceneBuildIndex_MainMenu;
            
            return LoadSceneAsync(sceneBuildIndex);
        }
        
        public AsyncOperation LoadNextLevelAsync(PlayerData playerData)
        {
            if (playerData.levelNumber < 0)
            {
                playerData.levelNumber = 0;
            }
            else
            {
                playerData.levelNumber++;
            }
            
            var sceneBuildIndex = TryGetSceneBuildIndexByLevelNumber(playerData.levelNumber, out var buildIndex)
                ? buildIndex
                : SceneBuildIndex_MainMenu;
            
            return LoadSceneAsync(sceneBuildIndex);
        }
        
        public AsyncOperation LoadMainMenuAsync()
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