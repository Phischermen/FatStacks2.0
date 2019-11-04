using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem singleton;

    public static int combo;
    public static int score;
    public static int accumulatedBoxes;
    public static int accumulatedScore;

    public static float timer;
    public static float comboTime = 4f;

    public Image meter;
    public Text comboText;
    public Text scoreText;
    public Text accumulatedScoreText;

    private static string accumulatedScoreFormatString = "Total Boxes!!!\n{0}\nTotal Score!!!\n{1}";
    private static Coroutine comboCoroutine;
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
        singleton.scoreText.text = score.ToString();
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            singleton.meter.fillAmount = timer / comboTime;
        }
    }

    private void OnDisable()
    {
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }
        Score();
        HideAndResetCombo();
    }

    public static void IncrementComboAndAccumulateScore(int score, int boxes)
    {
        timer = comboTime;
        if (combo == 0)
        {
            comboCoroutine = singleton.StartCoroutine(Combo());
        }
        combo++;
        accumulatedScore += score;
        accumulatedBoxes += boxes;
        singleton.comboText.text = combo.ToString();
        singleton.accumulatedScoreText.text = string.Format(accumulatedScoreFormatString, accumulatedBoxes, accumulatedScore);
    }

    private static void Score()
    {
        score += accumulatedScore * combo;
        singleton.scoreText.text = score.ToString();
        Player.singleton.myPickup.gunController.AddAmmoToGun(ArsenalSystem.GunType.match, 1);
    }

    public static void BreakCombo()
    {
        if (combo == 0)
        {
            return;
        }
        singleton.StopCoroutine(comboCoroutine);
        Player.singleton.myPickup.gunController.AddAmmoToGun(ArsenalSystem.GunType.match, 1);
        HideAndResetCombo();
    }

    private static void HideAndResetCombo()
    {
        combo = accumulatedScore = accumulatedBoxes = 0;
        singleton.meter.gameObject.SetActive(false);
    }

    static IEnumerator Combo()
    {
        singleton.meter.gameObject.SetActive(true);
        yield return new WaitUntil(() => timer <= 0);
        Score();
        HideAndResetCombo();
    }
}
