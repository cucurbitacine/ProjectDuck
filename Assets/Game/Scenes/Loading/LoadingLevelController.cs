using System.Collections;
using Game.Levels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Scenes.Loading
{
    public class LoadingLevelController : LevelController
    {
        [SerializeField] private Slider loadingSlider;

        private IEnumerator Start()
        {
            if (loadingSlider)
            {
                loadingSlider.gameObject.SetActive(false);
            }

            var loading = string.IsNullOrWhiteSpace(Game.LoadingData.sceneName)
                ? Game.MainMenuAsync()
                : SceneManager.LoadSceneAsync(Game.LoadingData.sceneName, LoadSceneMode.Single);
            
            while (loading != null && !loading.isDone)
            {
                if (loading.progress > 0.9f)
                {
                    if (loadingSlider)
                    {
                        loadingSlider.gameObject.SetActive(true);
                        
                        loadingSlider.value = loading.progress;
                    }
                }
                
                yield return null;
            }
        }
    }
}
