using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [HideInInspector]
    public List<Box> originalBoxList;
    private ChildrenResetter resetter;
    public int threshold;
    [HideInInspector]
    public int quota;
    private int accumulatedScore;
    private int accumulatedBoxes;
    [HideInInspector]
    public bool puzzleWasReset = false;
    private bool solved = false;

    private void Start()
    {
        resetter = GetComponent<ChildrenResetter>();
        quota = transform.childCount - 2 - threshold;
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
