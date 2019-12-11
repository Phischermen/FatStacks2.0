using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ConversationTrigger : MonoBehaviour
{
    private static Dictionary<int, List<ConversationTrigger>> conversationTriggers;
    private static int uniqueKey = 0;

    [Tooltip("Some conversation triggers are meant to trigger the same conversation. Set those triggers so that they share the same key. If left as -1, this trigger will automatically be given a unique key")]
    public int key = -1;
    public DialogueManager.InteruptionMode interuptionMode;
    public Material triggeredMaterial;
    public bool triggered = false;

    public Conversation conversation;
    public Conversation[] additionalConversations;

    public UnityEvent onComplete;

    private void Start()
    {
        conversation.onComplete = onComplete;
        if (!Application.isEditor)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        if(conversationTriggers == null)
        {
            conversationTriggers = new Dictionary<int, List<ConversationTrigger>>();
        }
        bool notSupposedToHaveKey = (key == -1);
        if (key == -1)
        {
            key = (uniqueKey + 100);
            uniqueKey += 1;
        }
        if (conversationTriggers.ContainsKey(key))
        {
            if (notSupposedToHaveKey)
            {
                print("Conversation trigger dictionary already has key. " + key.ToString());
            }
        }
        else
        {
            conversationTriggers.Add(key, new List<ConversationTrigger>());
        }
        conversationTriggers[key].Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Trigger();
    }

    public void Trigger()
    {
        if (!triggered)
        {
            foreach (ConversationTrigger trigger in conversationTriggers[key])
            {
                trigger.triggered = true;
                trigger.GetComponent<MeshRenderer>().material = triggeredMaterial;
            }
            DialogueManager.PushConversation(conversation, interuptionMode);
            if (additionalConversations.Length > 0)
            {
                foreach (Conversation conversation in additionalConversations)
                {
                    DialogueManager.PushConversation(conversation, DialogueManager.InteruptionMode.doNotInterupt);
                }
            }
        }
    }

    public void DisableTrigger()
    {
        triggered = true;
    }
}
