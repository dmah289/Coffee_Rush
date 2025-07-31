using Coffee_Rush.UI.BaseSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Coffee_Rush.UI
{
    public class LoadingOpenGame : ALoadingPage
    {
        [Header("Configs")]
        [SerializeField] private string newSceneName = "Gameplay scene";
        private AsyncOperationHandle<SceneInstance> sceneLoadHandle;
        
        
        private void OnEnable()
        {
            StartLoading().Forget();
        }
        
        protected override async UniTaskVoid OnFullFillAmount()
        {
            sceneLoadHandle = Addressables.LoadSceneAsync(newSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, false);
            await UniTask.WaitUntil(() => sceneLoadHandle.IsDone);
            
            await UniTask.Delay(1000);
            
            await sceneLoadHandle.Result.ActivateAsync();
        }
    }
}