using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchPad : MonoBehaviour
{
    //Time that TouchPad is set inactive after release
    public float deadTime = 1.0f;
    //Bool used to lock down button during its set dead time
    private bool _deadTimeActive = false;

    //public Unity Events we can use in the editor and tie other functions to.
    public UnityEvent onTouched, onReleased;
    //public string tag we can assign to the touching collider
    public string colliderTag = "Finger";

    //Checks if the current collider entering is the Touch pad and sets off OnTouched event.
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == colliderTag && !_deadTimeActive)
        {
            onTouched?.Invoke();
            // Debug.Log("I have been pressed");
        }
    }

    //Checks if the current collider exiting is the Touch pad and sets off OnReleased event.
    //It will also call a Coroutine to make the button inactive for however long deadTime is set to.
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == colliderTag && !_deadTimeActive)
        {
            onReleased?.Invoke();
            StartCoroutine(WaitForDeadTime());
        }
    }

    //Locks button activity until deadTime has passed and reactivates button activity.
    IEnumerator WaitForDeadTime()
    {
        _deadTimeActive = true;
        yield return new WaitForSeconds(deadTime);
        _deadTimeActive = false;
    }
}


