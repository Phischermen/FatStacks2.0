using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDecal : MonoBehaviour
{
    public Projector oldDecal;
    public GameObject newDecal;
    public GameObject otherDecal;
    public AudioSource audioSource;
    

    public void Switch()
    {
        audioSource.Play();
        oldDecal.enabled = false;
        newDecal.SetActive(true);
        otherDecal.SetActive(true);
    }
}
