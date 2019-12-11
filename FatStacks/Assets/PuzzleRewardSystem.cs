using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRewardSystem : MonoBehaviour
{
    public static PuzzleRewardSystem singleton;
    public GameObject[] rewards;

    private static int i;

    public void Start()
    {
        if(singleton == null)
        {
            singleton = this;
        }
        else
        {
            DestroyImmediate(singleton);
            singleton = this;
        }
    }
    public static void SpawnReward(Vector3 location)
    {
        if(singleton?.rewards != null && i < singleton.rewards.Length)
        {
            Instantiate(singleton.rewards[i], location, Quaternion.identity);
            i++;
        }
        
    }
}
