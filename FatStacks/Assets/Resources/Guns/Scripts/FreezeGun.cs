﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeGun : Gun
{
    public override void fire1(Ray ray)
    {
        //RoomResetterInteraction.canReset = true;
        RaycastHit hit_info;
        bool object_found = Physics.Raycast(ray, out hit_info, float.MaxValue, LayerMask.GetMask("Default", "InteractSolid", "Helicopter"));
        Box box = hit_info.transform?.GetComponent<Box>();
        if (object_found && box != null)
        {
            playFireSound(box.Frozen ? 1 : 0);
            box.Frozen = !box.Frozen;
            //ammo -= 1;
        }
        
    }
    public override bool canFire()
    {
        return ammo > 0;
    }
}
