﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmoInteraction : Interaction
{
    
    public ArsenalSystem.GunType gun;
    public int amount;

    public override void Interact(Pickup pickup)
    {
        int oldAmount = amount;
        amount = pickup.gunController.AddAmmoToGun(gun,amount);
        if(amount == 0)
        {
            Destroy(gameObject);
        }else if(oldAmount == amount)
        {
            pickup.exception.FlashText(GetException(0), 3);
        }
    }
}
