using Coffee_Rush.UI.BaseSystem;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu
{
    public class MainMenuPage : MonoBehaviour, IPage
    {
        [SerializeField] private RectTransform[] tabs;
        
        // TODO : Register callback when the footer button is clicked
        
        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }
}