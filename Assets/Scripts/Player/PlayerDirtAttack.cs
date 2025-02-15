using UnityEngine;

[CreateAssetMenu(fileName = "Dirt Attack", menuName = "ProjectElemental/Attack/Dirt")]
public class PlayerDirtAttack : ScriptableObject
{
    //After an attack commences,
    //player tries to move to the closest enemy in leap time seconds =>
    //waits until damage delay passes => damage is dealt to any enemies in range for damage time seconds
    //=>exitDelay time passes=>player can attack but cannot move until exitTime passe
    //=>player can now move

    [SerializeField]
    private float _maxLeapDistance = 0f;

    [SerializeField]
    private float _leapTime = 0f;

    [SerializeField]
    private int _damage = 0;

    [SerializeField]
    private float _range = 0;

    [SerializeField]
    private float _damageDelay = 0;

    [SerializeField]
    private float _damageTime = 0;

    [SerializeField]
    private float _exitDelay = 0;

    [SerializeField]
    private float _exitTime = 0;

    [SerializeField]
    private Sprite[] _frames = null;

    public float MaxLeapDistance => _maxLeapDistance;
    public float LeapTime => _leapTime;

    public int Damage => _damage;

    public float Range => _range;

    public float DamageDelay => _damageDelay;

    public float DamageTime => _damageTime;

    public float ExitDelay => _exitDelay;

    public float ExitTime => _exitTime;

    public Sprite[] Frames => _frames;
}