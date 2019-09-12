﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunPellet : Projectile
{
    private void OnCollisionEnter(Collision collision)
    {
        HealthManager healthManager = collision.gameObject.GetComponentInParent<HealthManager>();
        if(healthManager != null)
        {
            healthManager.DealDamage(damage,ownerHealthManager);
        }
        Destroy(gameObject);
    }

    public override void Hit(GameObject obj)
    {
        base.Hit(obj);
        HealthManager healthManager = obj.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            healthManager.DealDamage(damage);
        }
    }
}
