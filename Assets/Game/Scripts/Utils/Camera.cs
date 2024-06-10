using UnityEngine;

namespace Game.Scripts.Utils
{
    public static class CameraUtils
    {
        private static Camera _mainCamera;

        public static Camera Main
        {
            get
            {
                if (_mainCamera != null) return _mainCamera;
                
                _mainCamera = Camera.main;
                return _mainCamera;
            }
        }
    }
}