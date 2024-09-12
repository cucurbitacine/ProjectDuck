using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core
{
    /// <summary>
    /// Game Behaviour Controller
    /// </summary>
    public sealed class GameManager
    {
        #region Singleton

        public static GameManager Instance { get; private set; }

        static GameManager()
        {
            Instance = new GameManager();
        }
        
        #endregion
        
        private const string KeyName_PlayerData = nameof(PlayerData);
        
        public const int TotalNumberLevels = 2;
        private const int SceneBuildIndex_MainMenu = 0;
        private const int SceneBuildIndex_FirstLevel = 1;
        private const int SceneBuildIndex_CreditsScene = SceneBuildIndex_FirstLevel + TotalNumberLevels;
        
        private PlayerData PlayerData { get; set; }
        
        #region Public API

        /// <summary>
        /// Loading Player Data from <see cref="PlayerPrefs"/>. If something goes wrong, it will be created by default
        /// </summary>
        /// <returns></returns>
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
        
        /// <summary>
        /// Saving Player Data in <see cref="PlayerPrefs"/>
        /// </summary>
        /// <param name="playerData"></param>
        public async Task SavePlayerDataAsync(PlayerData playerData)
        {
            var jsonData = JsonUtility.ToJson(playerData);
            
            PlayerPrefs.SetString(KeyName_PlayerData, jsonData);
            
            Debug.Log($"[SAVED PLAYER] \"{playerData}\" as \"{jsonData}\"");
            
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Getting Player Data. If it doesn't exist, it will be loaded
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Resetting Player Data.  
        /// </summary>
        public void ResetPlayerData()
        {
            PlayerPrefs.DeleteKey(KeyName_PlayerData);
            
            PlayerData = null;

            Debug.Log($"[RESET PLAYER]");
        }
        
        /// <summary>
        /// Starting Game for Player Data 
        /// </summary>
        /// <param name="playerData"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Loading next Scene for Player Data
        /// </summary>
        /// <param name="playerData"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Loading The Main menu
        /// </summary>
        /// <returns></returns>
        public AsyncOperation LoadMainMenuAsync()
        {
            return LoadSceneAsync(SceneBuildIndex_MainMenu);
        }

        /// <summary>
        /// Quitting game
        /// </summary>
        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
        #endregion
        
        #region Private API
        
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
        
        
        
        private AsyncOperation LoadSceneAsync(int sceneBuildIndex)
        {
            Debug.Log($"[LOADING SCENE] \"{sceneBuildIndex}\" {PlayerData}");
            
            return SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);
        }

        #endregion
    }
}