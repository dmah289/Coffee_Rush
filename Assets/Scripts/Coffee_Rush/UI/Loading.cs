using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace Coffee_Rush.UI
{
    public class Loading : MonoBehaviour
    {
        private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");

        [Header("Components")]
        [SerializeField] private Image fillImg;
        
        [Header("Configs")]
        private bool newSceneLoading;
        [SerializeField] private string newSceneName = "GamePlay";
        private AsyncOperationHandle<SceneInstance> sceneLoadHandle;
        
        private void OnEnable()
        {
            StartLoading().Forget();
        }

        private async UniTaskVoid StartLoading()
        {
            float timer = 0f;

            while (timer < 3.5f)
            {
                timer += Time.deltaTime * UnityEngine.Random.Range(0.5f, 1.5f);
                fillImg.material.SetFloat(FillAmount, timer / 3.5f);
                await UniTask.Yield();
            }
            
            fillImg.material.SetFloat(FillAmount, 1f);
            
            sceneLoadHandle = Addressables.LoadSceneAsync(newSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, false);
            await UniTask.WaitUntil(() => sceneLoadHandle.IsDone);
            
            await UniTask.Delay(1000);
            
            await sceneLoadHandle.Result.ActivateAsync();
        }
    }
}