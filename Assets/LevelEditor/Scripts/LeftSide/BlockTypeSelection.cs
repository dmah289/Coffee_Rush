using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.Scripts.LeftSide
{
    public class BlockTypeSelection : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Button[] blockTypeButtons;

        [Header("Prefabs")] 
        [SerializeField] private GameObject[] blockPrefabs;
        
        private void Awake()
        {
            blockTypeButtons = GetComponentsInChildren<Button>();
            
            for(int i = 0; i < blockTypeButtons.Length; i++)
            {
                int index = i;
                blockTypeButtons[i].onClick.AddListener(() => OnBlockTypeSelected(index));
            }
        }

        private void OnBlockTypeSelected(int index)
        {
            Instantiate(blockPrefabs[index], Vector3.zero, Quaternion.identity);
        }
    }
}