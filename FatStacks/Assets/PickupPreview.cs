using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PickupPreview : MonoBehaviour
{
    public GameObject previewObject;

    public Image symbol;
    public Image symbolBackground;
    public Image prompt;
    public Image promptBackground;
    public Image baseCross;

    public Sprite[] promptSprites;
    public Sprite char1;
    public Sprite char2;

    private Color boxColor;
    public enum Prompt
    {
        e,r,f,g,ef,rg
    };

    public void SetPreview(Box box, Prompt _prompt)
    {
        //Set colors
        boxColor = box.color;
        SetColors(boxColor);
        //Set icon
        symbol.sprite = box.icon;
        //Set prompt
        promptBackground.sprite = (_prompt >= Prompt.ef) ? char2 : char1;
        prompt.sprite = promptSprites[(int)_prompt];
    }

    public void SetValid(bool valid)
    {
        SetColors(valid ? boxColor : Color.gray);
    }

    private void SetColors(Color color)
    {
        symbolBackground.color = promptBackground.color = baseCross.color = color;
    }
}
