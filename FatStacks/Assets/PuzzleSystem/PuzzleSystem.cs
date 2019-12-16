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
    private static Coroutine resetCoroutine;

    public static bool Reseting { get; private set; }

    private static readonly string laurelString = "<size=40>Puzzle Solved!!!</size>";
    private static readonly string resetString = "<size=40>Exit Area To Confirm Reset.</size>\nMouse3 to cancel Reset";
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
        puzzle.ShowBoundry(false);
        
        if(laurelCoroutine != null) singleton.StopCoroutine(laurelCoroutine);
        if(resetCoroutine != null) singleton.StopCoroutine(resetCoroutine);
        Reseting = false;
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

    public static void ResetPuzzle(Puzzle puzzle)
    {
        if(Reseting == false)
        {
            ShowUI(true);
            resetCoroutine = singleton.StartCoroutine(ResetPuzzleRoutine(puzzle));
        }
    }

    static IEnumerator Laurel()
    {
        yield return new WaitUntil(()=> !singleton.source.isPlaying);
        ShowUI(false);
    }

    static IEnumerator ResetPuzzleRoutine(Puzzle puzzle)
    {
        Reseting = true;
        puzzle.ShowBoundry(true);
        singleton.laurelText.text = resetString;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => puzzle.IsInPuzzleBoundry(Player.singleton.transform.position) == false || Input.GetButtonDown("Fire3"));
        if (!Input.GetButtonDown("Fire3"))
        {
            puzzle.ResetPuzzle();
        }
        ShowUI(false);
        puzzle.ShowBoundry(false);
        Reseting = false;
    }
}
