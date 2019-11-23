using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Conversation", menuName = "Dialogue/Conversation")]

public class Conversation : ScriptableObject
{
    [SerializeField]
    private Dialogue[] dialogue = new Dialogue[0];
    public int progress { get; private set; } = 0;

    public void Advance() { progress = Mathf.Min(progress + 1, dialogue.Length); }
    public void SetProgress(int p) { progress = Mathf.Clamp(p, 0, dialogue.Length); }
    public bool Done() { return progress == dialogue.Length; }
    public Dialogue GetCurrent() { return (progress < dialogue.Length) ? dialogue[progress] : null; }
}
