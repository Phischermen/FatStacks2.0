using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerIndoorAmbiance : MonoBehaviour
{
    bool triggered = false;
    public AmbiantTrack indoorAmbiance;
    public MusicTrack indoorMusic;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            triggered = true;
            MusicManager.singleton.PlayAmbiance(indoorAmbiance);
            MusicManager.singleton.PlayTrack(indoorMusic);
        }
        
    }
}
