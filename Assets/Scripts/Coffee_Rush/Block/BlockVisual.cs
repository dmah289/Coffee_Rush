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
        
        [Header("Movement Direction")]
        [SerializeField] private SpriteRenderer verticalSprite;
        [SerializeField] private SpriteRenderer horizontalSprite;
        
        [Header("References")]
        [SerializeField] private BLockMatcher blockMatcher;

        public int IceCountDown
        {
            get
            {
                if(int.TryParse(countdownTxt.text, out int countdown))
                    return countdown;

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    blockMatcher.CanSelect = false;
                    ice.SetActive(true);
                    countdownTxt.text = value.ToString();
                }
                else
                {
                    blockMatcher.CanSelect = true;
                    ice.SetActive(false);
                    countdownTxt.text = string.Empty;
                }
            }
        }
        
        private void Awake()
        {
            selfTransform = transform;
            selfTransform.localEulerAngles = BlockConfig.initEulerModel;

            blockMatcher = GetComponentInParent<BLockMatcher>();
        }

        public void OnBlockColected()
        {
            IceCountDown--;
        }


        public Vector3 EulerRotation
        {
            get => selfTransform.localEulerAngles;
            set => selfTransform.localEulerAngles = value;
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