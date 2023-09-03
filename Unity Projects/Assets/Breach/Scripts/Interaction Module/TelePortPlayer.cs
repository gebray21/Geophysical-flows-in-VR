using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelePortPlayer : MonoBehaviour
{
    //refence to the player transform
    [SerializeField] private GameObject Player;
    //refence to the destination transform
    [SerializeField] private Transform nextDestination;


    private void Awake()
    {
       if (!Player)
         Player = GameObject.FindGameObjectWithTag("Player");
    }
    public void Teleportplayer()
    {
        if(nextDestination!=null)
        {
            StartCoroutine(TeleportThePlayer());
            //Debug.Log("your are at " + Player.transform.position);
        }
        
    }

    IEnumerator TeleportThePlayer()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 nextPos= nextDestination.position + new Vector3(-1, 1, -1) * 1.0f;
        Quaternion rotation = nextDestination.rotation;
        //Player.transform.position = nextDestination.transform.position + new Vector3(-1, 1, -1) * 1.0f;
        Player.transform.position = nextPos;
        Player.transform.rotation = rotation;
        yield return null;
    }
}
