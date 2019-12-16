using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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

    private LineRenderer boundryLineRenderer;
    private Bounds boundry;
    public UnityEvent onImpossible;

    private void Start()
    {
        resetter = GetComponent<ChildrenResetter>();
        boundryLineRenderer = GetComponent<LineRenderer>();
        
        quota = transform.childCount - 1;

        //Populate line renderer
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size * 0.5f;
        boundryLineRenderer.positionCount = 4;
        boundryLineRenderer.loop = true;
        boundryLineRenderer.SetPosition(0, new Vector3(center.x + size.x, center.y - size.y, center.z + size.z));
        boundryLineRenderer.SetPosition(1, new Vector3(center.x + size.x, center.y - size.y, center.z - size.z));
        boundryLineRenderer.SetPosition(2, new Vector3(center.x - size.x, center.y - size.y, center.z - size.z));
        boundryLineRenderer.SetPosition(3, new Vector3(center.x - size.x, center.y - size.y, center.z + size.z));

        //Set bounds
        boundry = boxCollider.bounds;
        boundry.Expand(0.1f);
    }

    public void ShowBoundry(bool show)
    {
        boundryLineRenderer.enabled = show;
    }

    public bool IsInPuzzleBoundry(Vector3 location)
    {
        return boundry.Contains(location);
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

    public bool CheckIsPossible(bool doubleCheckAndInvokeEvent = false)
    {
        if (doubleCheckAndInvokeEvent) StopAllCoroutines();
        List<Box> boxes = new List<Box>();
        //Populate list of Boxes in puzzle
        boxes.AddRange(GetComponentsInChildren<Box>());
        //Check number of children
        if(boxes.Count < 3 && boxes.Count > 0)
        {
            Debug.Log("Not enough boxes" + boxes.Count.ToString());
            if (doubleCheckAndInvokeEvent) StartCoroutine(DoubleCheck());
            return false;
        }
        else if(boxes.Count == 0)
        {
            //Puzzle is actually solved. Technically not possible or impossible in this state, but return true to avoid invoking onImpossible
            Debug.Log("All boxes cleared!");
            return true;
        }
        //Find and count all liftable crates per group
        int i = 0;
        int[] group = new int[4] { 0, 0, 0, 0 };
        while(i < boxes.Count)
        {
            Box box = boxes[i];
            if (!box.isTooHeavy)
            {
                group[(int)box.groupId] += 1;
                if (group[(int)box.groupId] == 3)
                {
                    Debug.Log("Enough liftable boxes");
                    return true;
                }
                boxes.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
        //Check each metal box
        while (boxes.Count > 0)
        {
            Box box = boxes[0];
            List<Box> neighbors = box.GetMatchingNeighbors();
            //Does box have more than three neighbors or is there enough liftables to make three neighbors
            if (neighbors.Count + group[(int)box.groupId] >= 3)
            {
                Debug.Log("Enough liftable boxes + metal boxes");
                return true;
            }
            else
            {
                foreach (Box neighbor in neighbors)
                {
                    boxes.Remove(neighbor);
                }
            }
        }
        //On completion, return false
        Debug.Log("No matches possible");
        if (doubleCheckAndInvokeEvent) StartCoroutine(DoubleCheck());
        return false;
    }

    public void ResetPuzzle()
    {
        if (!solved)
        {
            puzzleWasReset = true;
            resetter.ResetRoom();
            foreach (Box box in originalBoxList)
            {
                Destroy(box.gameObject);
            }
            accumulatedBoxes = accumulatedScore = 0;
        }
    }

    IEnumerator DoubleCheck()
    {
        yield return new WaitForSeconds(3f);
        if (CheckIsPossible() == false)
        {
            onImpossible.Invoke();
        }
    }

    public void AccumulateBoxesAndScore(int boxes, int score)
    {
        accumulatedBoxes += boxes;
        accumulatedScore += score;
    }
}
