using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager singleton;

    private bool ignore = false;
    private bool endMusic;

    public MusicTrack currTrack { get; private set; } = null;
    public MusicTrack nextTrack { get; private set; } = null;

    AmbiantTrack currAmbiance = null;

    private Coroutine musicRoutine;
    private Coroutine ambianceRoutine;

    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource ambiantSource;

    public enum TrackVariant
    {
        cut,
        overlap,
        tail
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!singleton)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void PlayTrack(MusicTrack track)
    {
        musicSource.Stop();
        nextTrack = null;
        currTrack = track;
        if (musicRoutine != null)
            StopCoroutine(musicRoutine);
        musicRoutine = StartCoroutine(ManageMusic());
    }

    public void QueueUpNextTrack(MusicTrack track)
    {
        nextTrack = track;
        if (track == null)
        {
            //Get music to stop
            endMusic = true;
            ChangeTrack(currTrack, TrackVariant.tail);
        }
    }

    public void ChangeTrack(MusicTrack track, TrackVariant trackVariant = TrackVariant.overlap)
    {
        if (musicSource.isPlaying)
        {
            int startTime = musicSource.timeSamples;
            currTrack = track;
            AudioClip clip = null;
            switch (trackVariant)
            {
                case TrackVariant.cut:
                    clip = track.cut;
                    break;
                case TrackVariant.overlap:
                    clip = track.overlap;
                    break;
                case TrackVariant.tail:
                    clip = track.tail;
                    break;
            }
            musicSource.clip = clip;
            musicSource.timeSamples = startTime;
            ignore = true;
        }
        else
        {
            Debug.Log("Could not change music track because source was not playing.");
        }
    }

    public void PlayAmbiance(AmbiantTrack track)
    {
        ambiantSource.Stop();
        currAmbiance = track;
        if (ambianceRoutine != null)
            StopCoroutine(ambianceRoutine);
        ambianceRoutine = StartCoroutine(ManageAmbiance());
    }

    public void ChangeAmbiance(AmbiantTrack track)
    {
        if (ambiantSource.isPlaying)
        {
            int startTime = ambiantSource.timeSamples;
            currAmbiance = track;
            ambiantSource.clip = track.clip;
            ambiantSource.timeSamples = startTime;
        }
        else
        {
            Debug.Log("Could not change ambiant track because source was not playing.");
        }
    }

    public IEnumerator ManageMusic()
    {
        bool looped = false;
        while (true)
        {
            yield return new WaitUntil(() => musicSource.isPlaying == false);
            if (!ignore)
            {
                if (nextTrack != null)
                {
                    musicSource.clip = nextTrack.cut;
                    looped = false;
                    currTrack = nextTrack;
                    nextTrack = null;
                }
                else
                {
                    if (looped == false)
                    {
                        musicSource.clip = currTrack.cut;
                    }
                    else
                    {
                        musicSource.clip = currTrack.overlap;
                    }
                    looped = true;
                    musicSource.timeSamples = 0;
                }
            }
            ignore = false;
            if (endMusic)
            {
                endMusic = false;
                StopCoroutine(musicRoutine);
            }
            musicSource.Play();
        }
    }

    public IEnumerator ManageAmbiance()
    {
        bool looped = false;
        while (true)
        {
            yield return new WaitUntil(() => ambiantSource.isPlaying == false);
            if (!ignore)
            {
                if (nextTrack != null)
                {
                    ambiantSource.clip = ambiantSource.clip;
                    looped = false;
                }
                else
                {
                    if (looped == false)
                    {
                        ambiantSource.clip = currAmbiance.clip;
                    }
                    else
                    {
                        ambiantSource.clip = currAmbiance.clip;
                    }
                    looped = true;
                    ambiantSource.timeSamples = 0;
                }
            }
            ignore = false;
            ambiantSource.Play();
        }
    }
}
