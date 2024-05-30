using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    public class ModelLoader : MonoBehaviour
    {
        [SerializeField] private GameObject modelDefault;
        [SerializeField] private AssetReference modelReference;

        private AsyncOperationHandle<GameObject> modelInstantiateHandle;

        public event Action<GameObject> OnModelLoaded;

        public GameObject GetModel()
        {
            return modelInstantiateHandle.IsValid() && modelInstantiateHandle.IsDone
                ? modelInstantiateHandle.Result
                : modelDefault;
        }

        private async Task LoadModel()
        {
            modelInstantiateHandle = modelReference.InstantiateAsync(transform, false);

            await modelInstantiateHandle.Task;

            modelDefault.SetActive(false);
            
            OnModelLoaded?.Invoke(modelInstantiateHandle.Result);
        }

        private void UnloadModel()
        {
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