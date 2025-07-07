using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Coffee_Rush.Block
{
    public class PieceController : MonoBehaviour
    {
        [SerializeField] private float zScreenPos;
        
        private void Awake()
        {
            zScreenPos = Camera.main.WorldToScreenPoint(new Vector3(0, 0, -1f)).z;
        }

        private void OnMouseDown()
        {
            print("Block clicked: " + gameObject.name);
        
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = zScreenPos;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            transform.position = worldPos;
        }

        private void OnMouseDrag()
        {
            
            Debug.Log("Dragging block: " + gameObject.name);
        }
    }
}