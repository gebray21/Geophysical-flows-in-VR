using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// This script will toggle a GameObject whenever the provided InputAction is executed
/// </summary>
public class ToggleGameObject : MonoBehaviour
{
    [SerializeField] private InputActionReference InputAction = null;
    [SerializeField] private GameObject ToggleObject = null;
    private void Awake()
    {
        InputAction.action.started += ToggleActive;
    }
  

    private void OnDestroy()
    {
        InputAction.action.started -= ToggleActive;
    }

    public void ToggleActive(InputAction.CallbackContext context)
    {
        if (ToggleObject)
        {
            bool isActive = !ToggleObject.activeSelf;
            ToggleObject.SetActive(isActive);
            //Debug.Log("menu pressed");
        }
    }
}

