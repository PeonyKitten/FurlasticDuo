using UnityEngine;
using UnityEngine.InputSystem;

namespace FD.UI
{
    public class ControlsUIToggle : MonoBehaviour
    {
        [SerializeField] private GameObject controls;
        [SerializeField] private InputAction toggleControlsInputAction;

        private bool _uiEnabled = true;
        
        private void Start()
        {
            toggleControlsInputAction.Enable();
            toggleControlsInputAction.performed += ToggleUIControls;
        }

        private void ToggleUIControls(InputAction.CallbackContext obj)
        {
            _uiEnabled = !_uiEnabled;
            controls.SetActive(_uiEnabled);
        }

        private void OnDestroy()
        {
            toggleControlsInputAction.performed -= ToggleUIControls;
        }
    }
}
