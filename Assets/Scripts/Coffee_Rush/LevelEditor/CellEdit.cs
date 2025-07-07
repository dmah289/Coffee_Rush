using System;
using UnityEngine;

namespace Coffee_Rush.LevelEditor
{
    public class CellEdit : MonoBehaviour
    {
        private bool isMouseClicked;
        private void OnMouseEnter()
        {
            if (isMouseClicked)
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