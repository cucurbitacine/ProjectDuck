using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Scripts.Player
{
    [DisallowMultipleComponent]
    public class ModelLoader : MonoBehaviour
    {
        [SerializeField] private AssetReference modelReference;

        [Header("Default")]
        [SerializeField] private bool useDefault = false;
        [SerializeField] private GameObject modelDefault;
        
        private AsyncOperationHandle<GameObject> modelInstantiateHandle;

        public event Action<GameObject> OnModelLoaded;

        public GameObject GetModel()
        {
            if (useDefault) return modelDefault;
            
            return modelInstantiateHandle.IsValid() && modelInstantiateHandle.IsDone
                ? modelInstantiateHandle.Result
                : modelDefault;
        }

        private async Task LoadModel()
        {
            if (useDefault)
            {
                OnModelLoaded?.Invoke(modelDefault);
                
                return;
            }
            
            modelInstantiateHandle = modelReference.InstantiateAsync(transform, false);

            await modelInstantiateHandle.Task;

            modelDefault.SetActive(false);
            
            OnModelLoaded?.Invoke(modelInstantiateHandle.Result);
        }

        private void UnloadModel()
        {
            if (useDefault) return;
            
            Addressables.ReleaseInstance(modelInstantiateHandle);
        }

        private async void Start()
        {
            await LoadModel();
        }

        private void OnDestroy()
        {
            UnloadModel();
        }
    }
}