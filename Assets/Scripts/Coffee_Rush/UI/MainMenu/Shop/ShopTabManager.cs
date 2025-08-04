using System;
using Coffee_Rush.UI.BaseSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Coffee_Rush.UI.Shop
{
    public class ShopTabManager : MonoBehaviour
    {
        [SerializeField] private RectTransform noAds_bg;

        private void Awake()
        {
            AutoRotateNoadsBg();
        }

        private async UniTask AutoRotateNoadsBg()
        {
            while (true)
            {
                Vector3 currentRotation = noAds_bg.rotation.eulerAngles;
                currentRotation.z += 20 * Time.deltaTime;
                noAds_bg.rotation = Quaternion.Euler(currentRotation);
            
                // Wait for next frame
                await UniTask.Yield();
            }
        }
    }
}