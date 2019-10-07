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
    public static float comboTime = 1.5f;

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

    public static void BreakCombo()
    {
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
        while(timer > 0)
        {
            timer -= 0.01f;
            singleton.meter.fillAmount = timer / comboTime;
            yield return new WaitForSeconds(0.01f);
        }
        score += accumulatedScore * combo;
        singleton.scoreText.text = score.ToString();
        Player.singleton.myPickup.gunController.AddAmmoToGun(ArsenalSystem.GunType.match, 1);
        HideAndResetCombo();
    }
}
