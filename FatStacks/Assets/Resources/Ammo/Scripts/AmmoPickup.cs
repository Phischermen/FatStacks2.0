using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Interaction
{
    public GameObject model;
    public AudioSource audiosource;
    public ArsenalSystem.GunType gun;
    public int amount;
    public string logEntry;

    private bool notPickedUp = true;

    public override void Interact(Pickup pickup)
    {
        //do nothing
    }

    private void OnTriggerEnter(Collider other)
    {
        if(notPickedUp && other.gameObject == Player.singleton.gameObject)
        {
            notPickedUp = false;
            Pickup pickup = other.gameObject.GetComponentInChildren<Pickup>();
            int oldAmount = amount;
            amount = pickup.gunController.AddAmmoToGun(gun, amount);
            string formattedEntry = string.Format(logEntry, oldAmount - amount);
            ItemLog.AddItem(formattedEntry);
            if (amount == 0)
            {
                Destroy(gameObject, 5f);
                Destroy(model);
                audiosource.Play();
            }
            else if (oldAmount == amount)
            {
                pickup.exception.FlashText(GetException(0), 3);
            }
        }
    }
}
