using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    bool triggered = false;

    public Conversation conversation;
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            DialogueManager.PushConversation(conversation);
        }
        triggered = true;
    }
}
