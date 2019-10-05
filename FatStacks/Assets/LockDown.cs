using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDown : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player.singleton)
        {
            StartCoroutine(StartLockDown());
        }
    }

    IEnumerator StartLockDown()
    {
        Debug.Log("Lockdown");
        return null;
        //Queue Doors
        //Queue Lights
        //Queue Music
        //Queue UI
        //Queue Spawners
        //WaitUntil Target reached or Player fails
        //if player fails
            //Queue GameOver
            //Stop Music
        //Else
        //Unlock doors
        //Restore lights, music, doors etc.
    }
}
