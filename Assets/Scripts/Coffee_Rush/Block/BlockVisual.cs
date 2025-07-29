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
        [SerializeField] private Transform blockModel;
        
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
        
        private void Awake()
        {
            selfTransform = transform;
            selfTransform.localEulerAngles = BlockConfig.initEulerModel;
            blockModel = transform.GetChild(0);
        }

        public void OnBlockColected()
        {
            IceCountDown--;
        }
        
        public Vector3 VisualEulerAngle
        {
            set => selfTransform.localEulerAngles = value;
        }
        
        public Vector3 BlockModelEulerAngle
        {
            set => blockModel.transform.localEulerAngles = value;
        }

        public void ShowDirectionSprite(eMovementDirection direction)
        {
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