using UnityEngine;

public class DialogueEntry : ScriptableObject
{
    public enum Side
    {
        Left, 
        Right 
    };

    private string _text;
    private Side _portraitSide;

    public string Text => _text;
    public Side Portraitide => _portraitSide;
}
