using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInRelease : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
