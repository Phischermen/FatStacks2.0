using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public DialogueManager.InteruptionMode interuptionMode;

    public Conversation conversation;
    private void OnTriggerEnter(Collider other)
    {
        DialogueManager.PushConversation(conversation, interuptionMode);
    }
}
