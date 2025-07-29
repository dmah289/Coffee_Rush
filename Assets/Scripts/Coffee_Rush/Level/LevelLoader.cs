using System.Collections;
using Cysharp.Threading.Tasks;
using Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Coffee_Rush.Level
{
    public class LevelLoader : MonoBehaviour
    {
        public LevelData currLevelData;
        [SerializeField] private int levelIdx;

        public async UniTask LoadCurrentLevel()
        {
            int levelIndex = PlayerPrefs.GetInt(KeySave.LevelIndexKey, 0);
            AsyncOperationHandle<LevelData> levelLoaderHandle = Addressables.LoadAssetAsync<LevelData>($"Level {levelIdx}");

            await levelLoaderHandle;
            currLevelData = levelLoaderHandle.Result;
        }
    }
}