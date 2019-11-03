using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStart : MonoBehaviour
{
    // Basically just kill myself
    void Start()
    {
        Destroy(gameObject);
    }
}
