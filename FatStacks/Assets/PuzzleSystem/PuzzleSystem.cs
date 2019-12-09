using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PuzzleSystem : MonoBehaviour
{
    public static PuzzleSystem singleton;
    public Text laurelText;
    public AudioSource source;

    private static Coroutine laurelCoroutine;

    private static readonly string laurelString = "<size=40>Puzzle Solved!!!</size>";
    private static readonly string failiureString = "<size=40>Puzzle Impossible.</size>\nMouse2 to reset";
    private static readonly string scoreFormatString = "\n<size=15>Points: {0}</size>";
    private static readonly string hundredPercentClearBonusString = "\n<size=15>Perfect Clear Bonus: +1000</size>";
    private static readonly string noResetBonusString = "\n<size=15>No Reset Bonus: +1000</size>";

    // Start is called before the first frame update
    void Start()
    {
        if (!singleton)
        {
            singleton = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
        ShowUI(false);
    }

    public static void ShowUI(bool show = true)
    {
        singleton.gameObject.SetActive(show);
    }

    public static void Score(Puzzle puzzle)
    {
        ShowUI(true);
        singleton.source.Play();
        puzzle.solved = true;
        
        if(puzzle.reward != null)
        {
            Instantiate(puzzle.reward, puzzle.spawnLocation);
        }
        
        if(laurelCoroutine != null) singleton.StopCoroutine(laurelCoroutine);
        laurelCoroutine = singleton.StartCoroutine(Laurel());
        int points = 0;
        singleton.laurelText.text = laurelString;
        singleton.laurelText.text += string.Format(scoreFormatString, puzzle.accumulatedScore);
        points += puzzle.accumulatedScore;
        //if (puzzle.accumulatedBoxes == puzzle.quota + puzzle.threshold)
        //{
        //    singleton.laurelText.text += hundredPercentClearBonusString;
        //    points += 1000;
        //}
        if (!puzzle.puzzleWasReset)
        {
            singleton.laurelText.text += noResetBonusString;
            points += 1000;
        }
        ComboSystem.AddPoints(points);
    }

    static IEnumerator Laurel()
    {
        yield return new WaitUntil(()=> !singleton.source.isPlaying);
        ShowUI(false);
    }
}
