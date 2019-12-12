using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [TextArea(5, 10)]
    public string tutorialHeader;
    
    protected Fade tutorialUITextFader;
    protected TutorialTextController tutorialTextController;

    protected bool triggered = false;
    [HideInInspector]
    public bool ready = false;
    [HideInInspector]
    public bool done = false;
    public bool repeatable = false;
    public UnityEvent onComplete;

    // Start is called before the first frame update
    public virtual void Start()
    {
        GetUIComponents(GameObject.FindGameObjectWithTag("Player"));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (triggered == false && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TriggerTutorial();
        }
    }
    public virtual void TriggerTutorial()
    {
        triggered = true;
        tutorialTextController.pushTutorial(this);
    }
    public void GetUIComponents(GameObject Player)
    {
        GameObject tutorialUI = Player.transform.Find("UI/Tutorial").gameObject;
        tutorialUITextFader = tutorialUI.GetComponent<Fade>();
        tutorialTextController = tutorialUI.GetComponent<TutorialTextController>();
    }
    public virtual void ResetTutorial()
    {
        ready = done = triggered = false;
    }
}
