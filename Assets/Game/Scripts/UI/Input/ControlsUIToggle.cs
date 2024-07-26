using UnityEngine;
using UnityEngine.InputSystem;

namespace FD.UI.Input
{
    public class ControlsUIToggle : MonoBehaviour
    {
        [SerializeField] private GameObject controls;
        [SerializeField] private InputAction toggleControlsInputAction;
        [SerializeField] private bool uiEnabled = false;
        
        private void Start()
        {
            toggleControlsInputAction.Enable();
            toggleControlsInputAction.performed += ToggleUIControls;

            controls.SetActive(uiEnabled);
        }

        private void ToggleUIControls(InputAction.CallbackContext obj)
        {
            uiEnabled = !uiEnabled;
            controls.SetActive(uiEnabled);
        }

        private void OnDestroy()
        {
            toggleControlsInputAction.performed -= ToggleUIControls;
        }
    }
}
