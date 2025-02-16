using GsKit.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GsTextUGUI))]
public class DialogueController : MonoBehaviour
{
    private static DialogueController s_Instance = null;

    [SerializeField]
    private Image _leftImage;
    
    [SerializeField]
    private Image _rightImage;


    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private DialogueResource _dialogue = null;

    [SerializeField]
    private IEnemy[] _enemies;

    private GsTextUGUI _text;
    private TextMeshProUGUI _textUGUI;

    private int _currentEntry = 0;
    private bool _playing = false;

    public static DialogueController Instance => s_Instance == null ? 
        new GameObject().AddComponent<DialogueController>() : s_Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(s_Instance == null)
        {
            s_Instance = this;
            _text = GetComponent<GsTextUGUI>();
            _textUGUI = GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Destroy(s_Instance);
        }
    }

    void Start()
    {
        SetDialogue(_dialogue);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            NextEntry();
            return;
        }

        if (!_playing)
        {
            _text.enabled = false;
            _textUGUI.enabled = false;
            if (_leftImage != null)
            {
                _leftImage.enabled = false;
            }
            if (_rightImage != null)
            {
                _rightImage.enabled = false;
            }
        }
        if (_playing)
        {
            if (_text.VisibleCharacters < _text.GetParsedText().Length)
            {
                if (Input.GetButton("Fire1"))
                {
                    _text.OverrideTypewriteDelay(0.05f);
                }
                else
                {
                    _text.RestoreTypewriteDelay();
                }
            }
            else
            {
                if(Input.GetButtonDown("Fire1"))
                {
                    NextEntry();
                }
            }
        }
    }

    public void SetDialogue(DialogueResource resource) {
        _dialogue = resource;
        _currentEntry = 0;
        if (_dialogue != null && _dialogue.Entries.Count > 0)
        {
            _player.SetMovementControlsEnabled(false);
            _player.SetAttackControlsEnabled(false);

            foreach (IEnemy enemy in _enemies)
            {
                enemy.enabled = false;
            }
            _textUGUI.enabled = true;
            _text.enabled = true;
            if (_leftImage != null)
            {
                _leftImage.enabled = true;
            }
            if (_rightImage != null)
            {
                _rightImage.enabled = true;
            }
            UpdateEntry();

            _playing = true;
        }
    }
    
    private void NextEntry()
    {
        _currentEntry++;
        if (_currentEntry < _dialogue.Entries.Count)
        {
            UpdateEntry();
        }
        else
        {
            _player.SetMovementControlsEnabled(true);
            _player.SetAttackControlsEnabled(true);

            _playing = false;
        }
    }

    private void UpdateEntry() {
        DialogueEntryResource currentEntry = _dialogue.Entries[_currentEntry];

        _text.SetText(_dialogue.Entries[_currentEntry].Text);
        if ( _leftImage != null)
        {
            _leftImage.sprite = currentEntry.LeftPortrait;
        }
        if (_rightImage != null)
        {
            _rightImage.sprite = currentEntry.RightPortrait;
        }
    }
}
