using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
public class Press3D : MonoBehaviour
{
    public InputActionReference objectPressReference = null;
    public UnityEvent OnPress3D, OnRelease;
    public string gameObjectTag;
    public XRRayInteractor leftInteractor;
    // public XRRayInteractor rightInteractor;
    private RaycastHit res;
    private bool isPressed = false;
   

    // Start is called before the first frame update
    void Start()
    {
        objectPressReference.action.started += Selec3DObject;      
        
    }

    private void Update()
    {
        isPressed = RayHit(leftInteractor);
    }

    private void Selec3DObject(InputAction.CallbackContext ctx)
    {
        if(isPressed)
        OnPress3D?.Invoke();
        if (!isPressed)
            OnRelease?.Invoke();
    }

    private void OnDestroy()
    {
        objectPressReference.action.started -= Selec3DObject;
    }

    private bool RayHit(XRRayInteractor rayInteractor)
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
        {
            if (res.collider.gameObject.tag == "Pen")
            {
                return true;
            }
        }
        return false;
    }

}
