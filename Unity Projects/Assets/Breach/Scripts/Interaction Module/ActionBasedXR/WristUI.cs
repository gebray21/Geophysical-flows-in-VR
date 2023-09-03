using UnityEngine;
using UnityEngine.InputSystem;
public class WristUI : MonoBehaviour
{
    //public InputActionAsset inputActions;

    private Canvas _wristUICanvas;
   // private InputAction _menu;
    public InputActionReference menuReference = null;
    private void Start()
    {
        _wristUICanvas = GetComponent<Canvas>();
        // _menu = inputActions.FindActionMap("XRI LeftHand").FindAction("Menu");
        menuReference.action.Enable();
        menuReference.action.performed += ToggleMenu; 
        //  _menu.Enable();
        // _menu.performed += ToggleMenu;
    }

    private void OnDestroy()
    {
        // _menu.performed -= ToggleMenu;
        menuReference.action.performed -= ToggleMenu; 
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        _wristUICanvas.enabled = !_wristUICanvas.enabled;
       // Debug.Log("menu pressed");
    }
}



