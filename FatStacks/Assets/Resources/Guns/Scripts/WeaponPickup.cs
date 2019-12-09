using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class WeaponPickup : Interaction
{
    public ArsenalSystem.GunType gun;
    public int ammo;
    public string logEntry;
    public UnityEvent onPickup;

    public override void Interact(Pickup pickup)
    {
        //do nothing
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ItemLog.AddItem(logEntry);
            ArsenalSystem arsenal = other.gameObject.GetComponentInChildren<ArsenalSystem>();
            arsenal.AddGunToArsenalAndEquip(gun);
            arsenal.AddAmmoToGun(gun, ammo);
            onPickup.Invoke();
            Destroy(gameObject);
        }
    }
}
