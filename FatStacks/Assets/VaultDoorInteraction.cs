using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultDoorInteraction : DoorInteraction
{
    private static VaultDoorInteraction singleton;
    public ConversationTrigger conversation;

    private static int keys;
    public static void AddKey()
    {
        keys += 1;
        if (keys == 3)
        {
            singleton.conversation.DisableTrigger();
            singleton.isBusy = false;
            singleton.TwistLock(false);
        }
    }
    private new void Update()
    {
        //empty
    }
    public override void Interact(Pickup obj)
    {
        base.Interact(obj);
        isBusy = true;
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        if(singleton == null)
        {
            singleton = this;
        }
        isBusy = true;
        SwingDoor(false, false);
        TwistLock(true);
    }
}
