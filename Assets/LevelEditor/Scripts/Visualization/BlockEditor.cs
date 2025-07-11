using System;
using UnityEngine;

namespace LevelEditor.Scripts.Visualization
{
    public class BlockEditor : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        [SerializeField] private Rigidbody2D selfRb;
        
        [Header("Movement Settings")]
        private Vector3 centerToTouchOffset;
        private Camera cam;
        

        private void Awake()
        {
            selfTransform = transform;
            selfRb = selfTransform.GetComponent<Rigidbody2D>();
            
            cam = Camera.main;

            selfRb.isKinematic = true;
        }

        public void OnMouseDown()
        {
            centerToTouchOffset = cam.ScreenToWorldPoint(Input.mousePosition) - selfTransform.position;
            centerToTouchOffset.z = 0;
            
            selfRb.isKinematic = false;
        }

        public void OnMouseDrag()
        {
            Vector3 currTouchPos = cam.ScreenToWorldPoint(Input.mousePosition);
            currTouchPos.z = 0;
            
            Vector3 realtivelyTouchOffset = selfTransform.position + centerToTouchOffset;
            realtivelyTouchOffset.z = 0;

            Vector3 touchToInitTouchDir = (currTouchPos - realtivelyTouchOffset).normalized;
            
            float distance = Vector3.Distance(currTouchPos, realtivelyTouchOffset);

            if (distance > 3) distance = 3;
            else distance *= (distance / 2);
            
            selfRb.velocity = distance * 30 * touchToInitTouchDir;
        }

        private void OnMouseUp()
        {
            selfRb.velocity = Vector2.zero;
            selfRb.isKinematic = true;
        }
    }
}