using System.Collections;
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

        public IEnumerator LoadCurrentLevel()
        {
            int levelIndex = PlayerPrefs.GetInt(KeySave.LevelIndexKey, 0);
            AsyncOperationHandle<LevelData> levelLoaderHandle = Addressables.LoadAssetAsync<LevelData>($"Level {levelIdx}");

            yield return levelLoaderHandle;
            
            currLevelData = levelLoaderHandle.Result;
        }
    }
}