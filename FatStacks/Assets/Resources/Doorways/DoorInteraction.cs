﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : Interaction
{
    public BoxCollider obstructionDetection;
    private LayerMask mask = new LayerMask();
    [SerializeField]
    private bool locked;
    [SerializeField]
    private bool open;
    public bool isLocked { get; private set; }
    public bool isOpen { get; private set; }
    [HideInInspector]
    public Animator animator;
    protected void Start()
    {
        mask = LayerMask.GetMask("Player","InteractSolid");
        animator = GetComponent<Animator>();
        if (open)
        {
            SwingDoor(true, false);
        }
        TwistLock(locked);
    }
    public void Update()
    {
        isBusy = animator.IsInTransition(0);
    }
    public void TwistLock(bool locked)
    {
        isLocked = locked;
    }
    public void SwingDoor(bool open)
    {
        SwingDoor(open, true);
    }
    public void SwingDoor(bool open,bool animated = true)
    {
        animator.SetBool("open", open);
        if (!animated)
        {
            if(open)
                animator.Play("DoorOpen", 0, 1f);
            else
                animator.Play("DoorClose", 0, 1f);
        }
        isOpen = open;
        current_prompt = (isOpen) ? (1) : (0);
    }
    
    public override void Interact(Pickup obj)
    {
        if (!isBusy)
        {
            if (isLocked && !isOpen)
            {
                obj.exception.FlashText(GetException(0),3);
            }
            else if (!isLocked && !isOpen)
            {
                SwingDoor(true);
            }
            else if (isOpen)
            {
                Vector3 offset = transform.rotation * obstructionDetection.center;
                Vector3 pos = obstructionDetection.transform.position + offset;
                //Debug.DrawRay(pos, offset,Color.red,5f);
                
                if (!Physics.CheckBox(pos, obstructionDetection.size * 0.5f, transform.rotation, mask))
                {
                    SwingDoor(false);
                }
                else
                {
                    obj.exception.FlashText(GetException(1), 3);
                }
            }
        }
    }
}
