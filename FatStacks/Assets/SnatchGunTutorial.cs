using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnatchGunTutorial : MonoBehaviour
{
    [Header("UI")]
    public GameObject background;
    public GameObject tutorial1;
    public GameObject tutorial2;
    [Header("Conversations")]
    public ConversationTrigger work;
    public ConversationTrigger notWork;
    
    public void Trigger()
    {
        StartCoroutine(GiveTutorial());
    }

    IEnumerator GiveTutorial()
    {
        Player.singleton.UI.SetActive(false);
        background.SetActive(true);
        tutorial1.SetActive(true);
        yield return new WaitUntil(() => Input.GetButtonDown("Interact/Pickup"));
        yield return new WaitForSecondsRealtime(0.1f);
        tutorial1.SetActive(false);
        tutorial2.SetActive(true);
        yield return new WaitUntil(() => Input.GetButtonDown("Interact/Pickup"));
        tutorial2.SetActive(false);
        background.SetActive(false);
        Player.singleton.UI.SetActive(true);
        MatchGun matchGun = (MatchGun)Player.singleton.myPickup.gunController.arsenal[(int)ArsenalSystem.GunType.match]._gun;
        matchGun.onMatch.AddListener(work.Trigger);
        matchGun.onFailMatch.AddListener(notWork.Trigger);
    }
}
