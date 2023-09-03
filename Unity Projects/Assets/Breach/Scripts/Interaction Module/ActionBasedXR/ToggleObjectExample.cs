using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleObjectExample : MonoBehaviour
{
    //reference to our action reference that is the toggle action
    public InputActionReference toggleReference = null;
    //reference to teleport
    public InputAction teleportActivate = null;
   // public InputActionReference teleportReference;
    

    private void Awake()
    {
        toggleReference.action.started += Toggle;
        teleportActivate.Enable();
       // teleportReference.action.Enable();
    }

    private void OnDestroy()
    {
        toggleReference.action.started -= Toggle;
    }

    private void Toggle(InputAction.CallbackContext context)
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);
    }

    



   

   
    


    
}
