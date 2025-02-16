using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private DialogueResource _testDialogue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_testDialogue != null && Input.GetKeyDown(KeyCode.V))
        {
            DialogueController.Instance.SetDialogue(_testDialogue);
        }
    }
}
