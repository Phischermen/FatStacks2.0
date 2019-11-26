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
    static private Stack<int> progressStack;

    static private Text text;
    static private Image image;
    static private Image background;
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
        progressStack = new Stack<int>();
        text = GetComponentInChildren<Text>(true);
        image = GetComponentInChildren<Image>(true);
        background = GetComponentsInChildren<Image>(true)[1];
        source = GetComponentInChildren<AudioSource>(true);
    }

    public static void PushConversation(Conversation conversation, InteruptionMode interupt = InteruptionMode.doNotInterupt)
    {
        switch (interupt)
        {
            case InteruptionMode.doNotInterupt:
                conversationQueue.Enqueue(conversation);
                progressStack.Push(0);
                break;
            case InteruptionMode.interuptAndDoNotResume:
                if(conversationStack.Count > 0) conversationStack.Pop();
                if(progressStack.Count > 0) progressStack.Pop();
                progressStack.Push(0);
                conversationStack.Push(conversation);
                playConversationFromStackNow = true;
                break;
            case InteruptionMode.interuptAndResume:
                progressStack.Push(currentConversation.progress);
                conversationStack.Push(currentConversation);
                progressStack.Push(0);
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
            //Stop current dialogue
            source.Stop();
            if (currentConversation.Done() || playConversationFromStackNow)
            {
                playConversationFromStackNow = false;
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
                nextConversation.SetProgress(progressStack.Pop());
                currentConversation = nextConversation;

            }
            else
            {
                currentConversation.Advance();
            }
            if (!currentConversation.Done())
            {
                //Show
                ShowDialogueBox(true);
                Dialogue dialogue = currentConversation.GetCurrent();
                //Play the dialogue
                source.clip = dialogue.clip;
                source.Play();
                image.sprite = dialogue.icon;
                text.text = dialogue.dialogue;
                pause = dialogue.pause;
            }
            else
            {
                ShowDialogueBox(false);
            }
        }
    }

    void ShowDialogueBox(bool show = true)
    {
        image.gameObject.SetActive(show);
        background.gameObject.SetActive(show);
        text.gameObject.SetActive(show);
        source.gameObject.SetActive(show);
    }
}
