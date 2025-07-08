using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Coffee_Rush.Test
{
    public class Demo : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject go;
        GameObject instantiatedObject;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                go.InstantiateAsync().Completed += OnAssetInstantiated;
            if(Input.GetKeyDown(KeyCode.A))
                go.LoadAssetAsync().Completed += OnAssetLoaded;
            if(Input.GetKeyDown(KeyCode.R))
                // Just applied to the instantiated object by InstantiateAsync() method
                go.ReleaseInstance(instantiatedObject);
        }
        
        private void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                instantiatedObject = Instantiate(handle.Result);
            else Debug.LogError(handle.Status.ToString());
        }

        private void OnAssetInstantiated(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                instantiatedObject = handle.Result;
            else Debug.LogError(handle.Status.ToString());
        }
    }
}