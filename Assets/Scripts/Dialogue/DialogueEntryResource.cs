using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Entry", menuName = "ProjectElemental/Dialogue/Dialogue Entry")]
public class DialogueEntryResource : ScriptableObject
{
    [SerializeField]
    private string _text;

    public string Text => _text;
}
