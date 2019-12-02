using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private ChildrenResetter resetter;
    public int quota;
    private int accumulatedScore;
    private int accumulatedBoxes;
    private bool solved;

    private void Start()
    {
        resetter = GetComponent<ChildrenResetter>();    
    }

    public void Update()
    {
        if (!solved)
        {
            solved = (accumulatedBoxes > quota);
            if (solved)
            {
                Debug.Log("You solved the puzzle!");
            }
        }
        
    }

    public void ResetPuzzle()
    {
        if (!solved)
        {
            resetter.ResetRoom();
            accumulatedBoxes = accumulatedScore = 0;
        }
    }

    public void AccumulateBoxesAndScore(int boxes, int score)
    {
        accumulatedBoxes += boxes;
        accumulatedScore += score;
    }
}
