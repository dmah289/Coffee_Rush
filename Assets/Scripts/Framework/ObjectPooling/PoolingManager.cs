using System;
using System.Collections;
using Coffee_Rush.Block;
using Coffee_Rush.Board;
using Coffee_Rush.Obstacles;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private AssetReference[] blockerPrefabs;


        public bool IsInGamePoolingInitialized { get; private set; } = false;
        
        public async UniTask InitializeObjectInGamePooling()
        {
            // Load assets from Addressable and create pools
            AsyncOperationHandle<GameObject> tilePrefabHandle = Addressables.LoadAssetAsync<GameObject>(tilePrefab);
            await tilePrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.Tile, 2, tilePrefabHandle.Result.GetComponent<Tile>());
            
            AsyncOperationHandle<GameObject> outerCornerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(outerCornerPrefab);
            await outerCornerPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.OuterCorner, 2, outerCornerPrefabHandle.Result.GetComponent<ABorder>());
            
            AsyncOperationHandle<GameObject> straightBorderPrefabHandle = Addressables.LoadAssetAsync<GameObject>(straightBorderPrefab);
            await straightBorderPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.StraightBorder, 2, straightBorderPrefabHandle.Result.GetComponent<ABorder>());
            
            AsyncOperationHandle<GameObject> innerCornerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(innerCornerPrefab);
            await innerCornerPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.InnerCorner, 2, innerCornerPrefabHandle.Result.GetComponent<ABorder>());
            
            AsyncOperationHandle<GameObject> cupPrefabHandle = Addressables.LoadAssetAsync<GameObject>(cupPrefab);
            await cupPrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.GateItem, 2, cupPrefabHandle.Result.GetComponent<GateItem>());

            for (int i = 0; i < blockPrefabs.Length; i++)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(blockPrefabs[i]);
                await handle;
                ObjectPooler.SetUpPool((PoolingType)((byte)PoolingType.BlockType00 + i), 2, handle.Result.GetComponent<BlockController>());
                // Addressables.Release(handle);
            }
            
            AsyncOperationHandle<GameObject> gatePrefabHandle = Addressables.LoadAssetAsync<GameObject>(gatePrefab);
            await gatePrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.Gate, 2, gatePrefabHandle.Result.GetComponent<GateController>());
            
            AsyncOperationHandle<GameObject> kettlePrefabHandle = Addressables.LoadAssetAsync<GameObject>(kettlePrefab);
            await kettlePrefabHandle;
            ObjectPooler.SetUpPool(PoolingType.Kettle, 2, kettlePrefabHandle.Result.GetComponent<KettleController>());

            for (int i = 0; i < blockerPrefabs.Length; i++)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(blockerPrefabs[i]);
                await handle;
                ObjectPooler.SetUpPool((PoolingType)((byte)PoolingType.BlockerType00 + i), 2, handle.Result.GetComponent<BlockerController>());
                // Addressables.Release(handle);
            }

            IsInGamePoolingInitialized = true;
        }
    }
}