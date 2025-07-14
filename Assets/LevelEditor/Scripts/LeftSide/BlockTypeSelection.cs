#if UNITY_EDITOR
using System.Collections.Generic;
using LevelEditor.Scripts.Visualization;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.Scripts.LeftSide
{
    public class BlockTypeSelection : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Button[] blockTypeButtons;
        [SerializeField] private Image CanRemoveBtn;

        [Header("References")]
        [SerializeField] private HeaderLeftEditor headerLeftEditor;

        [Header("Blocks Manager")]
        [SerializeField] private BlockControllerEdit[] blockPrefabs;
        [SerializeField] private Transform blocksParent;
        public List<BlockControllerEdit> Blocks;

        [field: SerializeField] public bool CanRemove { get; private set; }


        private void Awake()
        {
            blockTypeButtons = GetComponentsInChildren<Button>();
            CanRemove = false;
            Blocks = new List<BlockControllerEdit>();
            
            for(int i = 0; i < blockTypeButtons.Length; i++)
            {
                int index = i;
                blockTypeButtons[i].onClick.AddListener(() => OnBlockTypeSelected(index));
            }
        }

        public void OnBlockTypeSelected(int index)
        {
            // if(!headerLeftEditor.IsLevelDataInitialized)
            //     throw new Exception("Board is not initialized. Please create board before selecting a block type.");
            
            BlockControllerEdit newBlock = Instantiate(blockPrefabs[index], Vector3.zero, Quaternion.identity, blocksParent);
            Blocks.Add(newBlock);
        }
        
        public void OnRemoveBtnClicked()
        {
            CanRemove = !CanRemove;
            if (CanRemove) CanRemoveBtn.color = Color.green;
            else CanRemoveBtn.color = Color.white;
        }
    }
}
#endif