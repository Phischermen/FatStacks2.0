using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTutorial : MonoBehaviour
{
    public InputTutorial reset;
    public MusicTrack track;

    public void Trigger()
    {
        MatchGun matchGun = (MatchGun)Player.singleton.myPickup.gunController.arsenal[(int)ArsenalSystem.GunType.match]._gun;
        matchGun.onFailMatch.AddListener(reset.TriggerTutorial);
        MusicManager.singleton.QueueUpNextTrack(track);
    }
}
