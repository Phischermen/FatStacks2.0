using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : Interaction
{
    public string logEntry;

    public override void Interact(Pickup pickup)
    {
        //do nothing
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ItemLog.AddItem(logEntry);
            VaultDoorInteraction.AddKey();
            Destroy(gameObject);
        }
    }
}
