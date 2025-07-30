using System;
using Coffee_Rush.Level;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Coffee_Rush.Block
{
    public class BlockVisual : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        [SerializeField] private GameObject ice;
        [SerializeField] private TextMeshPro countdownTxt;
        [SerializeField] private Transform visualParent;
        [SerializeField] private Transform colliderTransform;
        
        [Header("Movement Direction")]
        [SerializeField] private SpriteRenderer verticalSprite;
        [SerializeField] private SpriteRenderer horizontalSprite;

        private int curIceCountdown;
        public int IceCountDown
        {
            get => curIceCountdown;
            set
            {
                if (value > 0)
                {
                    curIceCountdown = value;
                    ice.SetActive(true);
                }
                else
                {
                    curIceCountdown = 0;
                    ice.SetActive(false);
                }
                countdownTxt.text = $"{curIceCountdown}";
            }
        }
        
        public Vector3 VisualEuler
        {
            get => visualParent.localEulerAngles;
            set
            {
                visualParent.localEulerAngles = value;
                colliderTransform.localEulerAngles = new Vector3(colliderTransform.localEulerAngles.x, colliderTransform.localEulerAngles.y, value.z);
            }
        }
        
        private void Awake()
        {
            selfTransform = transform;
            selfTransform.localEulerAngles = BlockConfig.initEulerModel;
        }

        public void OnBlockColected()
        {
            IceCountDown--;
        }

        public void ShowDirectionSprite(eMovementDirection direction)
        {
            if (Mathf.Approximately(VisualEuler.z, 90) || Mathf.Approximately(VisualEuler.z, 270))
            {
                if(direction == eMovementDirection.Horizontal) direction = eMovementDirection.Vertical;
                else if(direction == eMovementDirection.Vertical) direction = eMovementDirection.Horizontal;
            }
            
            if (direction == eMovementDirection.Both)
            {
                horizontalSprite.gameObject.SetActive(false);
                verticalSprite.gameObject.SetActive(false);
            }
            else if (direction == eMovementDirection.Horizontal)
            {
                horizontalSprite.gameObject.SetActive(true);
                verticalSprite.gameObject.SetActive(false);
            }
            else if (direction == eMovementDirection.Vertical)
            {
                horizontalSprite.gameObject.SetActive(false);
                verticalSprite.gameObject.SetActive(true);
            }
        }
    }
}