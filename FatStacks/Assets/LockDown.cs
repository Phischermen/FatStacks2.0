using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class LockDown : MonoBehaviour
{
    [Header("Box Sapwners")]
    public Transform boxGrid;
    public BoxSpawner[] spawners;
    public int boxLimit;
    [Header("Music Tracks")]
    public MusicTrack trackBuildUp;
    public MusicTrack trackNormal;
    public MusicTrack trackCloseCall;
    [Header("Queue Doors, Lights, etc.")]
    public UnityEvent onEnter;
    public UnityEvent onComplete;
    private bool triggered = false;

    private enum LockDownState
    {
        idle,
        preping,
        doing,
        finishing
    }

    public int originalBoxCount;
    public int liveBoxCount;
    public int boxReserveCount;
    private LockDownState state = LockDownState.idle;
    private bool spawnersEmpty = false;

    private void Start()
    {
        foreach (BoxSpawner spawner in spawners)
        {
            originalBoxCount += spawner.amount;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject == Player.singleton.gameObject)
        {
            triggered = true;
            state = LockDownState.preping;
            StartCoroutine(StartLockDown());
        }
    }

    private void Update()
    {
        if (triggered)
        {
            switch (state)
            {
                case LockDownState.doing:
                    boxReserveCount = 0;
                    liveBoxCount = boxGrid.childCount;
                    foreach (BoxSpawner spawner in spawners)
                    {
                        boxReserveCount += spawner.amount;
                    }
                    if (boxReserveCount == 0)
                    {
                        spawnersEmpty = true;
                    }
                    break;
            }
            LockdownSystem.UpdateUI(this);
        }
    }

    IEnumerator StartLockDown()
    {
        //Queue Doors
        //Queue Lights
        onEnter.Invoke();
        LockdownSystem.ShowUI();
        LockdownSystem.UpdateUI(this);
        //Queue Music
        //MusicManager.singleton.PlayTrack(trackBuildUp);
        //Queue UI
        //TODO show UI that tells player that they are in a lockdown.
        yield return new WaitForSeconds(3f);
        //Queue Spawners
        foreach(BoxSpawner spawner in spawners)
        {
            spawner.TurnSpawnerOn();
        }
        state = LockDownState.doing;
        yield return new WaitUntil(() => spawnersEmpty == true || liveBoxCount > boxLimit); //Target reached or Player fails
        state = LockDownState.finishing;
        LockdownSystem.ShowUI(false);
        //if player fails
        //Queue GameOver
        //Stop Music
        //MusicManager.singleton.PlayTrack(null);
        //Else
        //Unlock doors
        //Restore lights, music, doors etc.
        onComplete.Invoke();
    }
}
