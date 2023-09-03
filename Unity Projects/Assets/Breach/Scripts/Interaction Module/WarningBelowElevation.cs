using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class WarningBelowElevation : MonoBehaviour
{
   // public Transform Player;
   // public Collider boxCollider;
    [SerializeField]
    XRBaseController controller;
    Coroutine routine;
    private readonly Dictionary<Collider, Coroutine> _routines = new Dictionary<Collider, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Check if a routine already exists for this collide
            if (_routines.TryGetValue(other, out var routine) && routine != null)
            {
                // Avoid concurrent routines for the same object
                StopCoroutine(routine);
            }
            // start a routine for this collider and store the reference
            _routines[other] = StartCoroutine(StartPeriodicHaptics());
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Check if a routine already exists for this collider
            if (_routines.TryGetValue(other, out var routine) && routine != null)
            {
                // if yes stop it
                StopCoroutine(routine);
            }
        }

    }

    IEnumerator StartPeriodicHaptics()
    {
        // Trigger haptics every second
        var delay = new WaitForSeconds(1f);

        while (true) //when the player is in danger
        {
            yield return delay;
            SendHaptics();
        }
    }

    void SendHaptics()
    {
        if (controller != null)
            controller.SendHapticImpulse(0.7f, 0.5f);
    }

   

    bool PointInOABB(Vector3 point, BoxCollider box)
    {
        point = box.transform.InverseTransformPoint(point) - box.center;

        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);
        if (point.x < halfX && point.x > -halfX &&
           point.y < halfY && point.y > -halfY &&
           point.z < halfZ && point.z > -halfZ)
            return true;
        else
            return false;
    }
}
