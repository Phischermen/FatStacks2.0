using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public GameObject gear;
    public float initSpeed;
    public float Speed { get { return gear.GetComponent<Animator>().speed; } set { gear.GetComponent<Animator>().speed = value; } }

    private void Start()
    {
        Speed = initSpeed;
    }
};
