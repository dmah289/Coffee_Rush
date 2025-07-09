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
        [SerializeField] private AssetReference cellPrefab;
        [SerializeField] private AssetReference outerCornerPrefab;
        [SerializeField] private AssetReference straightBorderPrefab;


        public bool IsInGamePoolingInitialized { get; private set; } = false;
        
        public IEnumerator InitializeObjectInGamePooling()
        {
            // Load assets from Addressables and create pools
            AsyncOperationHandle<GameObject> cellPrefabHandle = Addressables.LoadAssetAsync<GameObject>(cellPrefab);
            yield return cellPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.Cell, 50, cellPrefabHandle.Result.GetComponent<Tile>());
            
            AsyncOperationHandle<GameObject> outerCornerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(outerCornerPrefab);
            yield return outerCornerPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.OuterCorner, 5, outerCornerPrefabHandle.Result.GetComponent<Transform>());
            
            AsyncOperationHandle<GameObject> straightBorderPrefabHandle = Addressables.LoadAssetAsync<GameObject>(straightBorderPrefab);
            yield return straightBorderPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.StraightBorder, 20, straightBorderPrefabHandle.Result.GetComponent<Transform>());
            
            
            // Unload the asset handles to free memory
            Addressables.Release(cellPrefabHandle);
            Addressables.Release(outerCornerPrefabHandle);
            Addressables.Release(straightBorderPrefabHandle);

            IsInGamePoolingInitialized = true;
        }
    }
}