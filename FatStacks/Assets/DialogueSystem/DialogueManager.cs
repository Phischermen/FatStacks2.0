using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    static public DialogueManager singleton;
    static private Conversation currentConversation;
    static private Stack<Conversation> conversationStack;
    static private Queue<Conversation> conversationQueue;

    static private bool playConversationFromStackNow;

    static private float pause;

    static private Text text;
    static private Image image;
    static private AudioSource source;

    public enum InteruptionMode{
        doNotInterupt,
        interuptAndDoNotResume,
        interuptAndResume,

    };
    void Start()
    {
        if(this != singleton)
        {
            if(singleton) DestroyImmediate(singleton);
            singleton = this;
        }
        currentConversation = new Conversation();
        conversationStack = new Stack<Conversation>();
        conversationQueue = new Queue<Conversation>();
        text = GetComponentInChildren<Text>();
        image = GetComponentInChildren<Image>();
        source = GetComponentInChildren<AudioSource>();
    }

    public static void PushConversation(Conversation conversation, InteruptionMode interupt = InteruptionMode.doNotInterupt)
    {
        switch (interupt)
        {
            case InteruptionMode.doNotInterupt:
                conversationQueue.Enqueue(conversation);
                break;
            case InteruptionMode.interuptAndDoNotResume:
                conversationStack.Pop();
                conversationStack.Push(conversation);
                playConversationFromStackNow = true;
                break;
            case InteruptionMode.interuptAndResume:
                conversationStack.Push(conversation);
                playConversationFromStackNow = true;
                break;
        }
        
    }

    private static bool ConversationsAreQueued()
    {
        return conversationStack.Count > 0 || conversationQueue.Count > 0;
    }

    private void Update()
    {
        if (!source.isPlaying)
        {
            pause = Mathf.Max(pause - Time.deltaTime, 0f);
        }
        if (playConversationFromStackNow || (!source.isPlaying && pause == 0f && (ConversationsAreQueued() || !currentConversation.Done())))
        {
            playConversationFromStackNow = false;
            //Stop current dialogue
            source.Stop();
            if (currentConversation.Done() || playConversationFromStackNow)
            {
                //Get next dialogue
                Conversation nextConversation = null;
                if (conversationStack.Count > 0)
                {
                    nextConversation = conversationStack.Pop();
                }
                else if (conversationQueue.Count > 0)
                {
                    nextConversation = conversationQueue.Dequeue();
                }
                currentConversation = nextConversation;
                currentConversation.Begin();
            }
            else
            {
                currentConversation.Advance();
            }
            if (currentConversation)
            {
                Dialogue dialogue = currentConversation.GetCurrent();
                //Play the dialogue
                source.clip = dialogue.clip;
                source.Play();
                image.sprite = dialogue.icon;
                text.text = dialogue.dialogue;
                pause = dialogue.pause;
            }
        }
    }
}
