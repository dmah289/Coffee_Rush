using System;
using Coffee_Rush.Level;
using Coffee_Rush.UI.InGame;
using DG.Tweening;
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
        [SerializeField] private GameObject kettle;
        [SerializeField] private TextMeshPro kettleTxt;
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
                bool hasIce = value > 0;
                curIceCountdown = hasIce ? value : 0;
                ice.SetActive(hasIce);
                countdownTxt.gameObject.SetActive(hasIce);
                
                countdownTxt.text = $"{curIceCountdown}";
            }
        }
        
        private int curKettleCountdown;

        public int KettleCountDown
        {
            get => curKettleCountdown;
            set
            {
                if (value == 0)
                {
                    if(curKettleCountdown > 0)
                    {
                        curKettleCountdown = 0;
                        kettle.transform.DOScale(1.5f, 0.7f).SetDelay(1f).OnComplete(() =>
                        {
                            LevelManager.Instance.ShowLoosePanel(eLooseReason.KettleExplosion);
                            kettle.SetActive(false);
                        });
                    }
                    else kettle.SetActive(false);
                }
                else
                {
                    curKettleCountdown = value;
                    kettle.SetActive(true);
                    kettle.transform.localScale = Vector3.one;
                }
                
                kettleTxt.text = $"{curKettleCountdown}";
            }
        }
        
        public Vector3 VisualEuler
        {
            get => visualParent.localEulerAngles;
            set
            {
                visualParent.localEulerAngles = value;
                
            }
        }

        public Vector3 ColliderEuler
        {
            set
            {
                colliderTransform.localEulerAngles = value;
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
            if(kettle.activeSelf)
                KettleCountDown--;
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

        public void TiltOnMoveOutOfView(float direction, float duration)
        {
            Vector3 targetEuler = new Vector3(0
                , 10 * direction
                , visualParent.localEulerAngles.z);
            visualParent.DOLocalRotate(targetEuler, duration);
        }

        public void HideKettle()
        {
            kettle.SetActive(false);
        }
    }
}