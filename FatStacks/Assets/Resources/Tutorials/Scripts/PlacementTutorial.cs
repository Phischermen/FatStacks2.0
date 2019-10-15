using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementTutorial : Tutorial
{

    private enum State
    {
        simple,
        advanced
    }
    State state;

    private void Start()
    {
        state = State.simple;
    }
    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            switch (state)
            {
                case State.simple:
                    break;
            }
        }
    }
}
