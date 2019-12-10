using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [HideInInspector]
    public List<Box> originalBoxList;
    private ChildrenResetter resetter;
    [HideInInspector]
    public int accumulatedScore;
    [HideInInspector]
    public int accumulatedBoxes;
    private int quota;
    [HideInInspector]
    public bool puzzleWasReset = false;
    [HideInInspector]
    public bool solved = false;

    private void Start()
    {
        resetter = GetComponent<ChildrenResetter>();
        quota = transform.childCount - 1;
    }

    public void Update()
    {
        CheckSolved();
    }

    public bool CheckSolved()
    {
        if (!solved)
        {
            solved = (accumulatedBoxes == quota);
            if (solved)
            {
                PuzzleSystem.Score(this);
            }
            return solved;
        }
        return false;
    }

    public void ResetPuzzle()
    {
        if (!solved)
        {
            puzzleWasReset = true;
            resetter.ResetRoom();
            foreach(Box box in originalBoxList)
            {
                Destroy(box.gameObject);
            }
            accumulatedBoxes = accumulatedScore = 0;
        }
    }

    public void AccumulateBoxesAndScore(int boxes, int score)
    {
        accumulatedBoxes += boxes;
        accumulatedScore += score;
    }
}
