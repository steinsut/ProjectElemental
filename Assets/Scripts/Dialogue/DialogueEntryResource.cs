using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Entry", menuName = "ProjectElemental/Dialogue/Dialogue Entry")]
public class DialogueEntryResource : ScriptableObject
{
    [SerializeField]
    private string _text;
    
    [SerializeField] 
    private Sprite _leftPortrait;

    [SerializeField]
    private Sprite _rightPortrait;

    public string Text => _text;

    public Sprite LeftPortrait => _leftPortrait;
    public Sprite RightPortrait => _rightPortrait;
}
