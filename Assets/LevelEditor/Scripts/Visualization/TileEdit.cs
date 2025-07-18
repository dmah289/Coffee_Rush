using System;
using Coffee_Rush.Block;
using Coffee_Rush.Board;
using UnityEngine;

namespace Coffee_Rush.LevelEditor
{
    public class TileEdit : MonoBehaviour
    {
        private bool isMouseClicked;
        
        [Header("LevelData")]
        public eBlockType occupiedBlockType;
        public eColorType blockColor;

        private void Awake()
        {
            occupiedBlockType = eBlockType.None;
        }

        private void OnMouseEnter()
        {
            if (isMouseClicked && Input.GetKey(KeyCode.LeftControl))
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                isMouseClicked = true;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                isMouseClicked = false;
            }
        }
    }
}