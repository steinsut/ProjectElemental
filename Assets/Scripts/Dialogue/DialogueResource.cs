using System.Collections.Generic;
using UnityEngine;

public class Dialogue : ScriptableObject
{
    [SerializeField]
    private List<DialogueEntry> _entries;

    public IReadOnlyList<DialogueEntry> Entries => _entries;
}
