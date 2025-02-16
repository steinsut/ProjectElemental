using UnityEngine;

public class DialogueEntry : ScriptableObject
{
    public enum Side
    {
        Left, 
        Right 
    };

    private string _dialogueText;
    private Side _portraitSide;
}
