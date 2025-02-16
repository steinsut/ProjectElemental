using GsKit.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D))]
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb2d;
    private Collider2D _collider;
    private Animator _animator;
    private Camera _mainCamera;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Quaternion _initialRotation;

    [SerializeField]
    private float _moveSpeed = 1.0f;

    [SerializeField]
    private float _dashTime = 1.0f;

    [SerializeField]
    private float _jumpSpeed = 1.0f;

    [SerializeField]
    private float _ascendingGravity = 1.0f;

    [SerializeField]
    private float _descendingGravity = 0.5f;

    [SerializeField]
    private float _jumpQueueSkinWidth = 0.05f;

    [SerializeField]
    private float _skinWidth = 0.01f;

    [SerializeField]
    private LayerMask _feetMask;

    [SerializeField]
    private ElementType _element = ElementType.NONE;

    [SerializeField]
    private LayerMask _enemyMask;

    [SerializeField]
    private Transform _mouseAnchor;

    [SerializeField]
    private Transform _projectileAnchor;

    [Header("Dirt Variant Details")]
    [SerializeField]
    private float _dirtMoveSpeed = 1.0f;

    [SerializeField]
    private PlayerDirtAttack[] _dirtAttacks;

    [Header("Fire Variant Details")]
    [SerializeField]
    private PlayerFireball _fireballPrefab;

    [SerializeField]
    private float _fireballCooldown = 1.0f;

    [SerializeField]
    private float _fireballCastTime = 1.0f;

    [SerializeField]
    private int _fireballDamage = 1;

    [SerializeField]
    private float _fireballSpeed = 1.0f;

    [SerializeField]
    private float _fireballSpeedDeviation = 0.25f;

    [Header("Air Variant Details")]
    [SerializeField]
    private float _doubleJumpSpeed = 1.0f;

    [SerializeField]
    private float _dashSpeed = 1.0f;

    [SerializeField]
    private Windblower _windblower;

    [SerializeField]
    private float _windforce = 1.0f;

    [Header("Water Variant Details")]
    [SerializeField]
    private float _waterbubbleRadius = 1.0f;

    [SerializeField]
    private float _waterbubbleStunDuration = 1.0f;

    [SerializeField]
    private float _waterbubbleCooldown = 1.0f;

    private bool _airborne = false;
    private bool _jumpQueued = false;
    private bool _hasDoubleJump = false;
    private bool _dashing = false;
    private bool _dashedOnce = false;
    private float _dashDelta = 0.0f;
    private float _dashSign = 1;

    private bool _canBeMoved = true;
    private bool _canAttack = true;

    private int _currentAttack;
    private Coroutine _currentAttackCoroutine;

    private bool _fireballOnCooldown = false;
    private bool _waterbubbleOnCooldown = false;

    private bool _stuckOnWall = false;

    public bool Controllable
    {
        get => _canBeMoved;
        set => _canBeMoved = value;
    }

    public ElementType Element
    {
        get => _element;
        set => _element = value;
    }

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCamera = Camera.main;
        _initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(_canBeMoved)
        {
            float vel = Input.GetAxis("Horizontal");
            if (vel < 0)
            {
                _spriteRenderer.flipX = true;
            }
            else if (vel > 0) {
                _spriteRenderer.flipX = false;
            }
            RaycastHit2D hit = Physics2D.Raycast(
                _collider.bounds.center + new Vector3(_collider.bounds.extents.x * Mathf.Sign(vel), 0, 0),
                transform.right * Mathf.Sign(vel),
                _skinWidth,
                _feetMask);
            if (!hit)
            {
                if(!_stuckOnWall)
                {
                    RaycastHit2D headHit = Physics2D.Raycast(
                        _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y, 0),
                        transform.up,
                        _skinWidth,
                        _feetMask);

                    if (_element == ElementType.WATER && headHit && Input.GetKey(KeyCode.LeftShift))
                    {
                        transform.up *= -1;
                        _rb2d.gravityScale = 0;
                        _stuckOnWall = true;
                    }
                    else
                    {
                        _rb2d.linearVelocityX = vel *
                            (_element == ElementType.DIRT ? _dirtMoveSpeed : _moveSpeed);
                        if(vel != 0 && !_airborne)
                        {
                            _animator.Play("Walk");
                        }
                        else {
                            _animator.Play("Idle");
                        }
                    }
                }
                else
                {
                    if(transform.right.y == 0)
                    {
                        _rb2d.linearVelocityY = 0;
                        _rb2d.linearVelocityX =  vel * _moveSpeed;
                    }
                    else
                    {
                        _rb2d.linearVelocityX = 0;
                        _rb2d.linearVelocityY = Input.GetAxis("Vertical") * _moveSpeed;
                    }
                }
            }
            else
            {
                if (_airborne && _element == ElementType.WATER && Input.GetKey(KeyCode.LeftShift))
                {
                    transform.right = transform.up * Mathf.Sign(vel);
                    _rb2d.gravityScale = 0;
                    _stuckOnWall = true;
                }
            }
        }

        if(_canAttack && Input.GetButtonDown("Fire1")) 
        {
            DoAttack();
            Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;
            _mouseAnchor.up = direction;
        }

        else if(!_canAttack && Input.GetButtonUp("Fire1"))
        {
            _windblower.Blowing = false;
        }

        RaycastHit2D groundHit;

        if(!_stuckOnWall)
        {
            groundHit = Physics2D.Raycast(
                _collider.bounds.center - new Vector3(0, _collider.bounds.extents.y, 0), 
                transform.up * -1, 
                _skinWidth,
                _feetMask);
        }
        else
        {
            if (transform.right.y == 0)
            {
                groundHit = Physics2D.Raycast(
                    _collider.bounds.center - new Vector3(0, -1 * Mathf.Sign(transform.right.x) * _collider.bounds.extents.y, 0),
                    transform.up * -1,
                    _skinWidth,
                    _feetMask);
            }
            else
            {
                groundHit = Physics2D.Raycast(
                    _collider.bounds.center - new Vector3(-1 * Mathf.Sign(transform.right.y) * _collider.bounds.extents.x, 0, 0),
                    transform.up * -1,
                    _skinWidth,
                    _feetMask);
            }
        }

        _airborne = !groundHit;

        if (!_airborne)
        {
            _hasDoubleJump = true;
            _dashedOnce = false;
            if(_canBeMoved && _element != ElementType.DIRT && (Input.GetButtonDown("Jump") || _jumpQueued))
            {
                _rb2d.linearVelocity = transform.up * _jumpSpeed;
                _jumpQueued = false;
                if(_stuckOnWall)
                {
                    transform.rotation = _initialRotation;
                    _rb2d.linearVelocityY = _doubleJumpSpeed;
                    _stuckOnWall = false;
                }
            }
        }
        else
        {
            if(_stuckOnWall)
            {
                _rb2d.linearVelocityY = _doubleJumpSpeed;
                transform.rotation = _initialRotation;
                _stuckOnWall = false;
            }

            RaycastHit2D jumpQueueHit = Physics2D.Raycast(
                _collider.bounds.center - new Vector3(0, _collider.bounds.extents.y, 0),
                 transform.up * -1,
                _jumpQueueSkinWidth,
                _feetMask);


            if (!_jumpQueued)
            {
                _jumpQueued = _canBeMoved && (bool)jumpQueueHit && Input.GetButtonDown("Jump");
            }

            if (_element == ElementType.AIR)
            {
                if (_canBeMoved && _hasDoubleJump && Input.GetButtonDown("Jump"))
                {
                    _rb2d.linearVelocityY = _doubleJumpSpeed;
                    _hasDoubleJump = false;
                    _jumpQueued = false;
                }
                if(_canBeMoved && 
                    !_dashedOnce &&
                    Input.GetKeyDown(KeyCode.LeftShift) &&
                    Input.GetAxisRaw("Horizontal") != 0)
                {
                    _dashing = true;
                    _dashedOnce = true;
                    _dashDelta = 0;
                    _dashSign = Mathf.Sign(Input.GetAxis("Horizontal"));
                }
                if(_dashing)
                {
                    _rb2d.linearVelocityX = _dashSpeed * _dashSign;
                    _rb2d.linearVelocityY = 0;
                    _rb2d.gravityScale = 0;
                    _dashDelta += Time.deltaTime;
                    if(_dashDelta > _dashTime)
                    {
                        _dashDelta = 0;
                        _dashing = false;
                        _rb2d.gravityScale = _descendingGravity;
                    }
                }
            }
            else
            {
                _dashing = false;
            }
            if(!_dashing && !_stuckOnWall)
            {
                if (_rb2d.linearVelocityY > 0)
                {
                    _rb2d.gravityScale = _ascendingGravity;
                }
                else
                {
                    _rb2d.gravityScale = _descendingGravity;
                }
            }
        }
        if(_windblower.Blowing) {
            Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;
            _mouseAnchor.up = direction;
        }
    }

    public void ToggleMovementControls()
    {
        _canBeMoved = !_canBeMoved;
    }

    public void ToggleAttackControls()
    {
        _canAttack = !_canAttack;
    }

    private IEnumerator DoDirtAttack(PlayerDirtAttack attack)
    {
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float sign = Mathf.Sign(mousePos.x - transform.position.x);
        RaycastHit2D[] hits = Physics2D.RaycastAll(
                _collider.bounds.center + new Vector3(_collider.bounds.extents.x * sign, 0, 0),
                Vector2.right * sign,
                attack.MaxLeapDistance,
                _enemyMask
                );
        _spriteRenderer.flipX = sign > 0 ? false : true;

        if (hits.Length > 0)
        {
            float currentVel = 0;
            while(transform.position.x < hits[0].point.x - (attack.Range / 2))
            {
                float newX = Mathf.SmoothDamp(transform.position.x, hits[0].point.x, ref currentVel, attack.LeapTime);
                transform.Translate(newX - transform.position.x, 0, 0);
                yield return null;
            }
        }

        _animator.Play(attack.StateName);
        yield return new WaitForSeconds(attack.DamageDelay);
        float delta = 0;
        List<int> ids = new();
        while (delta < attack.DamageTime)
        {
            RaycastHit2D[] damageHits = Physics2D.RaycastAll(
                    _collider.bounds.center + new Vector3(_collider.bounds.extents.x * sign, 0, 0),
                    Vector2.right * sign,
                    attack.Range,
                    _enemyMask
                    );

            foreach(RaycastHit2D hit in damageHits)
            {
                if(!ids.Contains(hit.collider.GetHashCode()))
                {
                    ids.Add(hit.collider.GetHashCode());
                    StartCoroutine(hit.collider.gameObject.GetComponent<IEnemy>().TakeDamage(attack.Damage));
                }
            }
            delta += Time.deltaTime;
        }
        yield return new WaitForSeconds(attack.ExitDelay);
        _canAttack = true;
        yield return new WaitForSeconds(attack.ExitTime);
        _canBeMoved = true;
        _animator.Play("Idle");
    }

    private IEnumerator DoFireball()
    {
        _fireballOnCooldown = true;
        yield return new WaitForSeconds(_fireballCastTime);

        float rand = Random.Range(0, 1f) ;

        PlayerFireball fireball = Instantiate<PlayerFireball>(_fireballPrefab, _projectileAnchor.position, Quaternion.identity);
        fireball.damage = _fireballDamage;
        fireball.transform.right = _mouseAnchor.up;
        fireball.velocity = (_fireballSpeed + rand * _fireballSpeedDeviation);
        fireball.velocityDeviation = rand;

        yield return new WaitForSeconds(_fireballCooldown);
        _fireballOnCooldown = false;
    }

    public void PushEnemy(IEnemy enemy) {
        RaycastHit2D hit = Physics2D.Raycast(
            _windblower.transform.position,
            enemy.transform.position - _windblower.transform.position,
            (enemy.transform.position - _windblower.transform.position).magnitude,
            _feetMask);

        if(!hit)
        {
            enemy.Push(_mouseAnchor.up * _windforce);
        }
    }

    private IEnumerator WaitWaterbubbleCooldown()
    {
        yield return new WaitForSeconds(_waterbubbleCooldown);
        _waterbubbleOnCooldown = false;
    }

    public void DoAttack()
    {
        switch(_element)
        {
            case ElementType.DIRT:
                _rb2d.linearVelocityX = 0;
                if(_dirtAttacks == null || _dirtAttacks.Length == 0) break;
                _canBeMoved = false;
                _canAttack = false;
                if (_currentAttackCoroutine != null)
                {
                    StopCoroutine(_currentAttackCoroutine);
                }
                _currentAttackCoroutine = StartCoroutine(DoDirtAttack(_dirtAttacks[_currentAttack % _dirtAttacks.Length]));
                _currentAttack++;
                break;
            case ElementType.FIRE:
                if (_fireballOnCooldown) break;
                if(_currentAttackCoroutine != null)
                {
                    StopCoroutine(_currentAttackCoroutine);
                }
                _currentAttackCoroutine = StartCoroutine(DoFireball());
                break;
            case ElementType.AIR:
                _windblower.Blowing = true;
                break;
            case ElementType.WATER:
                if (_waterbubbleOnCooldown) break;

                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    transform.position,
                    _waterbubbleRadius,
                    _enemyMask
                    );

                foreach (Collider2D hit in hits)
                {
                    StartCoroutine(hit.GetComponent<IEnemy>().Stun(_waterbubbleStunDuration));
                }
                if (_currentAttackCoroutine != null)
                {
                    StopCoroutine(_currentAttackCoroutine);
                }
                _waterbubbleOnCooldown = true;
                _currentAttackCoroutine = StartCoroutine(WaitWaterbubbleCooldown());
                break;
        }
    }
}
