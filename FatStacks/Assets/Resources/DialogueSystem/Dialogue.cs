using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName ="new Dialogue", menuName ="Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public Sprite icon;
    [TextArea(3,3)]
    public string dialogue;
    public AudioClip clip;
    public float pause = 0.1f;
}
