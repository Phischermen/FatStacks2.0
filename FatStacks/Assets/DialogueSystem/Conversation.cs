using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Conversation", menuName = "Dialogue/Conversation")]

public class Conversation : ScriptableObject
{
    [SerializeField]
    private Dialogue[] dialogue = new Dialogue[0];
    private int progress = 0;

    public void Begin() { progress = 0; }
    public void Advance() { progress = Mathf.Min(progress + 1, dialogue.Length - 1); }
    public bool Done() { return progress == dialogue.Length - 1 || dialogue.Length == 0; }
    public Dialogue GetCurrent() { return dialogue[progress]; }
}
