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

    private int boxCount;
    private LockDownState state = LockDownState.idle;
    private int spawnersBoxSupply = 0;
    private bool spawnersEmpty = false;

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
                    boxCount = boxGrid.childCount;
                    spawnersEmpty = true;
                    foreach (BoxSpawner spawner in spawners)
                    {
                        if(spawner.amount > 0)
                        {
                            spawnersEmpty = false;
                            break;
                        }
                    }
                    break;
            }
        }
    }

    IEnumerator StartLockDown()
    {
        //Queue Doors
        //Queue Lights
        onEnter.Invoke();
        //Queue Music
        //MusicManager.singleton.PlayTrack(trackBuildUp);
        //Queue UI
        //TODO show UI that tells player that they are in a lockdown.
        yield return new WaitForSeconds(3f);
        //Queue Spawners
        foreach(BoxSpawner spawner in spawners)
        {
            spawnersBoxSupply += spawner.amount;
            spawner.TurnSpawnerOn();
        }
        state = LockDownState.doing;
        yield return new WaitUntil(() => spawnersEmpty == true || boxCount > boxLimit); //Target reached or Player fails
        state = LockDownState.finishing;
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
