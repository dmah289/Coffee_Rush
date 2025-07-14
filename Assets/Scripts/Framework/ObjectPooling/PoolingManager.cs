using System;
using System.Collections;
using Coffee_Rush.Board;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.ObjectPooling
{
    
    
    public class PoolingManager : MonoBehaviour
    {
        [Header("InGame Asset Reference Prefabs")]
        [SerializeField] private AssetReference tilePrefab;
        [SerializeField] private AssetReference outerCornerPrefab;
        [SerializeField] private AssetReference straightBorderPrefab;
        [SerializeField] private AssetReference innerCornerPrefab;


        public bool IsInGamePoolingInitialized { get; private set; } = false;
        
        public IEnumerator InitializeObjectInGamePooling()
        {
            // Load assets from Addressable and create pools
            AsyncOperationHandle<GameObject> tilePrefabHandle = Addressables.LoadAssetAsync<GameObject>(tilePrefab);
            yield return tilePrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.Tile, 50, tilePrefabHandle.Result.GetComponent<Tile>());
            
            AsyncOperationHandle<GameObject> outerCornerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(outerCornerPrefab);
            yield return outerCornerPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.OuterCorner, 5, outerCornerPrefabHandle.Result.GetComponent<Transform>());
            
            AsyncOperationHandle<GameObject> straightBorderPrefabHandle = Addressables.LoadAssetAsync<GameObject>(straightBorderPrefab);
            yield return straightBorderPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.StraightBorder, 20, straightBorderPrefabHandle.Result.GetComponent<Transform>());
            
            AsyncOperationHandle<GameObject> innerCornerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(innerCornerPrefab);
            yield return innerCornerPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.InnerCorner, 5, innerCornerPrefabHandle.Result.GetComponent<Transform>());
            
            
            // Unload the asset handles to free memory
            Addressables.Release(tilePrefabHandle);
            Addressables.Release(outerCornerPrefabHandle);
            Addressables.Release(straightBorderPrefabHandle);
            Addressables.Release(innerCornerPrefabHandle);

            IsInGamePoolingInitialized = true;
        }
    }
}