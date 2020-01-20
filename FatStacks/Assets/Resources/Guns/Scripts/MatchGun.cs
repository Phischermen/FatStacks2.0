using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class MatchGun : Gun
{
    [HideInInspector]
    public UnityEvent onMatch;
    [HideInInspector]
    public UnityEvent onFailMatch;

    private Box targetedBox;
    private Vector3Int targetedBoxCoords;
    private List<Box> targetedGroup = new List<Box>();

    public override void scan(Ray ray)
    {
        base.scan(ray);
        RaycastHit hit_info;
        bool object_found = Physics.Raycast(ray, out hit_info, float.MaxValue, LayerMask.GetMask("Default", "InteractSolid", "Helicopter"));
        Box oldBox = targetedBox;
        
        if (object_found)
        {
            targetedBox = hit_info.transform.gameObject.GetComponent<Box>();
            if(targetedBox != null)
            {
                if ((targetedBox != oldBox && !targetedGroup.Find(b => b == targetedBox)) || BoxCoordDictionary.matchgunNeedsUpdate)
                {
                    BoxCoordDictionary.matchgunNeedsUpdate = false;
                    List<Box> newGroup = new List<Box>();
                    newGroup = targetedBox.GetMatchingNeighbors();
                    HighlightGroup(false, targetedGroup);
                    HighlightGroup(true, newGroup);
                    targetedGroup = newGroup;
                }
            }
            else
            {
                HighlightGroup(false, targetedGroup);
                targetedBox = null;
                targetedGroup.Clear();
            }
        }
        else
        {
            HighlightGroup(false, targetedGroup);
            targetedBox = null;
            targetedGroup.Clear();
        }
    }

    public void HighlightGroup(bool highlight, List<Box> group)
    {
        foreach (Box box in group)
        {
            if (box != null)
            {
                box.Highlight(highlight);
            }
        }
    }

    public override void fire1(Ray ray){
        base.fire1(ray);
        if (targetedGroup.Count > 0)
        {
            Puzzle puzzle = targetedBox.puzzle;
            int count = targetedGroup.Count;
            if (count >= 3)
            {
                onMatch.Invoke();
                int score = 0;
                int i = 0;
                bool puzzleReset = false;
                if (count < 6)
                {
                    playFireSound(0);
                    ammo -= 1;
                }
                else
                {
                    playFireSound(1);
                }
                foreach (Box item in targetedGroup)
                {
                    i += 1;
                    score += Mathf.Min(i , 6) * 10;
                    if(item.puzzle != null)
                    {
                        item.puzzle.AccumulateBoxesAndScore(1, 10);
                        if (item.puzzle.CheckSolved())
                        {
                            PuzzleRewardSystem.SpawnReward(item.GetComponent<Renderer>().bounds.center);
                        }
                        puzzleReset = puzzleReset || item.puzzle.puzzleWasReset;
                    }
                    Instantiate(item.boxData.destructionPrefab[(int)item.groupId], item.transform.position,Quaternion.identity);
                    Destroy(item.gameObject);
                    DestroyImmediate(item);
                }
                if (puzzle)
                {
                    puzzle.CheckIsPossible(true);
                }
                if (!puzzleReset)
                {
                    ComboSystem.IncrementComboAndAccumulateScore(score, count);
                }
                else
                {
                    ammo += 1;
                }
                targetedBox = null;
                targetedGroup.Clear();
            }
            else
            {
                onFailMatch.Invoke();
                playErrorSound(0);
                ComboSystem.BreakCombo();
            }
        }
        else
        {
            playErrorSound(0);
        }
    }
    public override void fire2(Ray ray)
    {
        //base.fire2(ray);
        //RaycastHit hit_info;
        //bool object_found = Physics.Raycast(ray, out hit_info, float.MaxValue, LayerMask.GetMask("Default", "InteractSolid", "Helicopter"));
        //if (object_found && hit_info.transform.GetComponent<Box>() != null)
        //{
        //    List<Box> boxs = hit_info.transform.gameObject.GetComponent<Box>().GetMatchingNeighbors();
        //    foreach(Box box in boxs)
        //    {
        //        box.Highlight();
        //    }
        //}
    }

    public override bool canFire()
    {
        return ammo > 0;
    }
}
