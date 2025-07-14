using UnityEngine;

namespace Framework.Helper
{
    public static class CameraHelper
    {
        private static Camera cam = Camera.main;
        
        public static Vector3 GetMouseWorldPosTitledCamera2D(float zPlane = 0)
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            // calculate distance from camera to zPlane
            float distance = Mathf.Abs((zPlane-cam.transform.position.z) / cam.transform.forward.z);
            mouseScreenPos.z = distance;
            
            Vector3 worldPos = cam.ScreenToWorldPoint(mouseScreenPos);
            worldPos.z = zPlane;
            return worldPos;
        }
    }
}