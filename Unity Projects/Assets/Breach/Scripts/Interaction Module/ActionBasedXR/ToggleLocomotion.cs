using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleLocomotion : MonoBehaviour
{
    public enum LocomotionType
    {
        Continuous,
        Noncontinous
    }
    public GameObject XRRigPlayer; //the game Object holding the movement scripts
    TeleportationProvider _teleport;
    ActionBasedContinuousMoveProvider _continuousMove;

    [Header("Locomotion Type")]
    /// <summary>
    /// Default locomotion to use, 0 = continous move. 1 = non continuous move
    /// </summary>
    [Tooltip("Default locomotion to use, 0 = continous move. 1 = non continuous move")]
    public LocomotionType DefaultLocomotion = LocomotionType.Continuous;
    public GameObject leftRayTeleport;
    public GameObject rightRayTeleport;
    LocomotionType selectedLocomotion = LocomotionType.Continuous;
    public LocomotionType SelectedLocomotion
    {
        get { return selectedLocomotion; }
    }
    
    public InputActionReference LocomotionToggleAction = null;
    private XRRayInteractor leftRay, rightRay;
    void Start()
    {
        _teleport = XRRigPlayer.GetComponent<TeleportationProvider>();
        _continuousMove = XRRigPlayer.GetComponent<ActionBasedContinuousMoveProvider>();
        leftRay = leftRayTeleport.GetComponent<XRRayInteractor>();
        rightRay = rightRayTeleport.GetComponent<XRRayInteractor>();
        //check in case both of them are active at a time
        if(DefaultLocomotion==LocomotionType.Continuous)
        {
            _teleport.enabled = false;
            _continuousMove.enabled = true;
            leftRayTeleport.SetActive(false);
            rightRayTeleport.SetActive(false);
        }
        else if (DefaultLocomotion == LocomotionType.Noncontinous)
        {
            _teleport.enabled = true;
            _continuousMove.enabled = false;
            leftRayTeleport.SetActive(true);
            leftRay.enabled = true;
            rightRayTeleport.SetActive(true);
            rightRay.enabled = true;
        }
          
    }

    

    bool actionToggle = false;
    
   /* void Update()
    {
        // Make sure we don't double toggle our inputs
        if (!actionToggle)
        {
            CheckControllerToggleInput();
        }

        actionToggle = false;
    }*/

    public virtual void CheckControllerToggleInput()
    {
        // if the LocomotionToggleAction is called 
        LocomotionToggle();                 
    }
    
    void OnEnable()
    {
        if (LocomotionToggleAction)
        {
            LocomotionToggleAction.action.performed -= OnLocomotionToggle; //unsubscribe first if any
            LocomotionToggleAction.action.Enable();
            LocomotionToggleAction.action.performed += OnLocomotionToggle;
        }
    }
      
    private void OnDestroy()
    {
        if(LocomotionToggleAction)
        {
            LocomotionToggleAction.action.Disable();
            LocomotionToggleAction.action.performed -= OnLocomotionToggle;
            Destroy(this.gameObject);
        }
    }
    
    void OnDisable()
    {
        if (LocomotionToggleAction)
        {
            LocomotionToggleAction.action.Disable();
            LocomotionToggleAction.action.performed -= OnLocomotionToggle;
        }
    }
   
    public void OnLocomotionToggle(InputAction.CallbackContext context)
    {
        actionToggle = true;
       // Debug.Log(SelectedLocomotion + "selected");
        LocomotionToggle();
        actionToggle = false;
    }

    public void LocomotionToggle()
    {


        // Toggle the locomotion

       ChangeLocomotion(SelectedLocomotion == LocomotionType.Continuous ? LocomotionType.Noncontinous : LocomotionType.Continuous);
       
    }

    public void ChangeLocomotion(LocomotionType locomotionType)
    {
        ChangeLocomotionType(locomotionType);
       // UpdateTeleportStatus();
    }
    public void ChangeLocomotionType(LocomotionType loc)
    {

        selectedLocomotion = loc;

        // Make sure Smooth Locomotion is available
        if (_continuousMove == null)
        {
            _continuousMove = XRRigPlayer.GetComponent<ActionBasedContinuousMoveProvider>();
        }

        if (_teleport == null)
        {
            _teleport = XRRigPlayer.GetComponent<TeleportationProvider>();
        }

        toggleTeleport(selectedLocomotion == LocomotionType.Noncontinous);
        toggleContinuousMove(selectedLocomotion == LocomotionType.Continuous);
    }

    void toggleContinuousMove(bool enabled)
    {
        if (_continuousMove)
        {
            _continuousMove.enabled = enabled;
        }
    }
    void toggleTeleport(bool enabled)
    {
        if (_teleport)
        {
            _teleport.enabled = enabled;
            leftRayTeleport.SetActive(enabled);
            rightRayTeleport.SetActive(enabled);
        }
    }
    public void UpdateTeleportStatus()
    {
        _teleport.enabled = SelectedLocomotion == LocomotionType.Noncontinous;
    }

    
        
    
}
