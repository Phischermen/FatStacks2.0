﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GunData gunData;

    public int ammo;
    protected AudioSource gunAudioSource;
    protected Animator gunAnimator;

    public void Start()
    {
        gunAudioSource = GetComponentInParent<AudioSource>();
        gunAnimator = GetComponent<Animator>();
        ammo = Mathf.Clamp(ammo, 0, gunData.ammoCapacity);
    }
    public virtual void fire1(Ray ray)
    {
        gunAnimator.SetTrigger("fire");
    }
    public virtual void fire2(Ray ray)
    {
        gunAnimator.SetTrigger("fire");
    }

    public virtual float getAmmoFill()
    {
        return Mathf.Clamp(ammo / (float)gunData.ammoCapacity, 0f, 1f);
    }

    public virtual bool canFire()
    {
        return true;
    }

    public virtual int addAmmo(int amount)
    {
        ammo = ammo + amount;
        int leftover = Mathf.Max(ammo - gunData.ammoCapacity, 0);
        ammo -= leftover;
        return leftover;
    }

    protected void playFireSound(int index)
    {
        gunAudioSource.clip = gunData.shootSound[index % gunData.shootSound.Length];
        gunAudioSource.Play();
    }

    protected void playEmptySound(int index)
    {
        gunAudioSource.clip = gunData.emptySound[index % gunData.shootSound.Length];
        gunAudioSource.Play();
    }

    protected void playErrorSound(int index)
    {
        gunAudioSource.clip = gunData.errorSound[index % gunData.shootSound.Length];
        gunAudioSource.Play();
    }
}
