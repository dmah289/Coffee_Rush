using Cysharp.Threading.Tasks;
using Framework;
using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Coffee_Rush.UI.MainMenu.SharedUI
{
    public class SettingButton : ScaleAnimButton
    {
        [SerializeField] private GameObject onImg;
        [SerializeField] private int selfIndex;

        protected override void Awake()
        {
            base.Awake();
            
            int curState = PlayerPrefs.GetInt(KeySave.SettingsKeys[selfIndex], 1);
            onImg.SetActive(curState == 1);
        }

        protected override async UniTaskVoid OnButtonClickedAsync()
        {
            int curState = PlayerPrefs.GetInt(KeySave.SettingsKeys[selfIndex], 1);
            int newState = 1 - curState;
            PlayerPrefs.SetInt(KeySave.SettingsKeys[selfIndex], newState);
            onImg.SetActive(newState == 1);
            
            base.OnButtonClickedAsync().Forget();
        }
    }
}