﻿using UnityEngine;

namespace FD.Utils
{
    public static class CameraUtils
    {
        private static Camera _mainCamera;

        public static void ResetMainCamera()
        {
            _mainCamera = null;
        }

        public static Camera Main
        {
            get
            {
                if (_mainCamera) return _mainCamera;
                
                _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        public static Vector2 CalculateRelativeMovement(this Camera primaryCamera, Vector2 input)
        {
            var cameraForward = primaryCamera.transform.forward;
            var cameraRight = primaryCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            var forwardRelativeInput = input.y * cameraForward;
            var rightRelativeInput = input.x * cameraRight;

            var cameraRelativeMovement = forwardRelativeInput + rightRelativeInput;
            
            return cameraRelativeMovement.Flatten();
        }
    }
}