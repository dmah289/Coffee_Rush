using System;
using System.Collections;
using Coffee_Rush.Block;
using Coffee_Rush.Board;
using Coffee_Rush.Obstacles;
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
        [SerializeField] private AssetReference cupPrefab;
        [SerializeField] private AssetReference[] blockPrefabs;
        [SerializeField] private AssetReference gatePrefab;
        [SerializeField] private AssetReference kettlePrefab;


        public bool IsInGamePoolingInitialized { get; private set; } = false;
        
        public IEnumerator InitializeObjectInGamePooling()
        {
            // Load assets from Addressable and create pools
            AsyncOperationHandle<GameObject> tilePrefabHandle = Addressables.LoadAssetAsync<GameObject>(tilePrefab);
            yield return tilePrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.Tile, 50, tilePrefabHandle.Result.GetComponent<Tile>());
            
            AsyncOperationHandle<GameObject> outerCornerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(outerCornerPrefab);
            yield return outerCornerPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.OuterCorner, 5, outerCornerPrefabHandle.Result.GetComponent<ABorder>());
            
            AsyncOperationHandle<GameObject> straightBorderPrefabHandle = Addressables.LoadAssetAsync<GameObject>(straightBorderPrefab);
            yield return straightBorderPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.StraightBorder, 5, straightBorderPrefabHandle.Result.GetComponent<ABorder>());
            
            AsyncOperationHandle<GameObject> innerCornerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(innerCornerPrefab);
            yield return innerCornerPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.InnerCorner, 5, innerCornerPrefabHandle.Result.GetComponent<ABorder>());
            
            AsyncOperationHandle<GameObject> cupPrefabHandle = Addressables.LoadAssetAsync<GameObject>(cupPrefab);
            yield return cupPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.GateItem, 5, cupPrefabHandle.Result.GetComponent<GateItem>());

            for (int i = 0; i < blockPrefabs.Length; i++)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(blockPrefabs[i]);
                yield return handle;
                ObjectPooler.SetUpPool((PoolingType)((byte)PoolingType.BlockType00 + i), 2, handle.Result.GetComponent<BlockController>());
                Addressables.Release(handle);
            }
            
            AsyncOperationHandle<GameObject> gatePrefabHandle = Addressables.LoadAssetAsync<GameObject>(gatePrefab);
            yield return gatePrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.Gate, 3, gatePrefabHandle.Result.GetComponent<GateController>());
            
            AsyncOperationHandle<GameObject> kettlePrefabHandle = Addressables.LoadAssetAsync<GameObject>(kettlePrefab);
            yield return kettlePrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.Kettle, 2, kettlePrefabHandle.Result.GetComponent<KettleController>());

            // yield return WaitHelper.GetWait(3f);
            
            // Unload the asset handles to free memory
            // Addressables.Release(tilePrefabHandle);
            // Addressables.Release(outerCornerPrefabHandle);
            // Addressables.Release(straightBorderPrefabHandle);
            // Addressables.Release(innerCornerPrefabHandle);
            // Addressables.Release(cupPrefabHandle);
            // Addressables.Release(gatePrefabHandle);
            

            IsInGamePoolingInitialized = true;
        }
    }
}