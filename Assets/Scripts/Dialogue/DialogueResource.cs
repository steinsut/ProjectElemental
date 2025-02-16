using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ProjectElemental/Dialogue/Dialogue")]
public class DialogueResource : ScriptableObject
{
    [SerializeField]
    private List<DialogueEntryResource> _entries;

    public IReadOnlyList<DialogueEntryResource> Entries => _entries;
}
