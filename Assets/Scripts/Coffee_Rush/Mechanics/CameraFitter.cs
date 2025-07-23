using System;
using Framework.Helper;
using UnityEngine;

namespace Coffee_Rush.Mechanics
{
    public class CameraFitter : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        public void CameraFitBoard(int boardWidthCoord)
        {
            mainCam.orthographicSize = 1.7f * boardWidthCoord + 5.5f;
        }
    }
}