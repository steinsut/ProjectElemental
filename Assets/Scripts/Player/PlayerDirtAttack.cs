using UnityEngine;

[CreateAssetMenu(fileName = "Dirt Attack", menuName = "ProjectElemental/Attack/Dirt")]
public class PlayerDirtAttack : ScriptableObject
{
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
    private string _stateName = "";

    public float MaxLeapDistance => _maxLeapDistance;
    public float LeapTime => _leapTime;

    public int Damage => _damage;

    public float Range => _range;

    public float DamageDelay => _damageDelay;

    public float DamageTime => _damageTime;

    public float ExitDelay => _exitDelay;

    public float ExitTime => _exitTime;

    public string StateName => _stateName;
}