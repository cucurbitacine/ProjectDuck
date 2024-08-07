using System;
using System.Collections;
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
        
        #endregion

        private const string ResourceName_LevelSchedule = "LevelScheduleDefault";
        
        private const string SceneName_MainMenu = "MainMenu";
        private const string KeyName_PlayerProfile = nameof(PlayerData);

        private LevelSchedule LevelSchedule { get; set; }
        
        public PlayerData PlayerData { get; private set; }

        public async Task LoadLevelSchedule()
        {
            if (LevelSchedule) return;

            //var timeout = Task.Delay(60000); // 1 min

            var loading = LoadingLevelSchedule();
            
            while (loading.MoveNext())
            {
                await Task.Delay(100);
            }
        }
        
        public async Task LoadPlayerDataAsync()
        {
            try
            {
                var jsonProfile = PlayerPrefs.GetString(KeyName_PlayerProfile);
                PlayerData = JsonUtility.FromJson<PlayerData>(jsonProfile) ?? CreateNewPlayerData();
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message} - {e.StackTrace}");
                
                PlayerData = CreateNewPlayerData();
            }
            
            await Task.CompletedTask;
            
            //Debug.LogWarning($"[LOADED] {JsonUtility.ToJson(PlayerData)}");
        }
        
        public async Task SavePlayerDataAsync()
        {
            if (PlayerData == null)
            {
                PlayerData = CreateNewPlayerData();
            }
            
            var jsonProfile = JsonUtility.ToJson(PlayerData);
            
            PlayerPrefs.SetString(KeyName_PlayerProfile, jsonProfile);
            
            await Task.CompletedTask;
            
            //Debug.LogWarning($"[SAVED] {jsonProfile}");
        }

        public async void ResetPlayerData()
        {
            PlayerData = CreateNewPlayerData();

            await SavePlayerDataAsync();
        }

        public AsyncOperation StartGameAsync()
        {
            if (PlayerData.currentLevel < 0)
            {
                PlayerData.currentLevel = 0;
            }
            
            var levelName = GetLevelName(PlayerData.currentLevel);
            
            return LoadSceneAsync(levelName);
        }
        
        public AsyncOperation LoadNextLevelAsync()
        {
            var nextLevelName = GetNextLevelName(PlayerData.currentLevel);

            PlayerData.currentLevel++;
            
            return LoadSceneAsync(nextLevelName);
        }
        
        public AsyncOperation MainMenuAsync()
        {
            return LoadSceneAsync(SceneName_MainMenu);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private IEnumerator LoadingLevelSchedule()
        {
            var loadingResource = Resources.LoadAsync<LevelSchedule>(ResourceName_LevelSchedule);

            yield return loadingResource;

            LevelSchedule = (LevelSchedule)loadingResource.asset;
        }
        
        private AsyncOperation LoadSceneAsync(string sceneName)
        {
            return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        
        private string GetLevelName(int currentLevel)
        {
            return LevelSchedule && LevelSchedule.TryGetLevelName(currentLevel, out var level)
                ? level
                : SceneName_MainMenu;
        }
        
        private string GetNextLevelName(int currentLevel)
        {
            return LevelSchedule && LevelSchedule.TryGetNextLevelName(currentLevel, out var nextLevel)
                ? nextLevel
                : SceneName_MainMenu;
        }
    }
}