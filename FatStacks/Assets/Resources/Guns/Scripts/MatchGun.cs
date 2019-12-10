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

    public override void fire1(Ray ray){
        RaycastHit hit_info;
        bool object_found = Physics.Raycast(ray, out hit_info, float.MaxValue, LayerMask.GetMask("Default", "InteractSolid", "Helicopter"));
        if (object_found && hit_info.transform.GetComponent<Box>() != null)
        {
            List<Box> boxs = hit_info.transform.gameObject.GetComponent<Box>().GetMatchingNeighbors();
            int count = boxs.Count;
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
                foreach (Box item in boxs)
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
                }
                if (!puzzleReset)
                {
                    ComboSystem.IncrementComboAndAccumulateScore(score, count);
                }
                else
                {
                    ammo += 1;
                }
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
        if(Player.singleton.myPickup.carriedObjects.Count > 0) //Don't reset anything if player is carrying boxes
        {
            return;
        }
        RaycastHit hit_info;
        bool object_found = Physics.Raycast(ray, out hit_info, float.MaxValue, LayerMask.GetMask("Default", "InteractSolid"));
        Puzzle puzzle = hit_info.transform?.GetComponent<Box>()?.puzzle;
        if (object_found && puzzle != null)
        {
            puzzle.ResetPuzzle();
        }
    }
    public override bool canFire()
    {
        return ammo > 0;
    }
}
