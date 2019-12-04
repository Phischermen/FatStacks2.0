using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LockdownSystem : MonoBehaviour
{
    public static LockdownSystem singleton;
    public Gradient boxAmountGradient;
    public Image amountFill;
    public Image progressFill;

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

    public static void UpdateUI(LockDown lockDown)
    {
        singleton.progressFill.fillAmount = (float)lockDown.boxReserveCount / lockDown.originalBoxCount;
        float a = (float)lockDown.liveBoxCount / lockDown.boxLimit;
        singleton.amountFill.fillAmount = a;
        singleton.amountFill.color = singleton.boxAmountGradient.Evaluate(a);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
