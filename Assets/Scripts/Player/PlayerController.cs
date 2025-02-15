using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D))]
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb2d;
    private Collider2D _collider;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Camera _mainCamera;

    [SerializeField]
    private float _moveSpeed = 1.0f;

    [SerializeField]
    private float _dashTime = 1.0f;

    [SerializeField]
    private float _dashSpeed = 1.0f;

    [SerializeField]
    private float _jumpSpeed = 1.0f;

    [SerializeField]
    private float _doubleJumpSpeed = 1.0f;

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
    private float _dirtMoveSpeed = 1.0f;

    [SerializeField]
    private PlayerDirtAttack[] _dirtAttacks;

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
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mainCamera = Camera.main;
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
            if(!hit)
            {
                _rb2d.linearVelocityX = vel *
                    (_element == ElementType.DIRT ? _dirtMoveSpeed : _moveSpeed);
            }
        }


        if(_canAttack && Input.GetAxisRaw("Fire1") > 0) 
        {
            _rb2d.linearVelocityX = 0;
            DoAttack();
        }

        RaycastHit2D groundHit = Physics2D.Raycast(
            _collider.bounds.center - new Vector3(0, _collider.bounds.extents.y, 0), 
            transform.up * -1, 
            _skinWidth,
            _feetMask);

        _airborne = !groundHit;
        if (!_airborne)
        {
            _hasDoubleJump = true;
            _dashedOnce = false;
            if(_canBeMoved && _element != ElementType.DIRT && (Input.GetButtonDown("Jump") || _jumpQueued))
            {
                _rb2d.linearVelocity = transform.up * _jumpSpeed;
                _jumpQueued = false;
            }
        }
        else
        {
            RaycastHit2D jumpQueueHit = Physics2D.Raycast(
                _collider.bounds.center - new Vector3(0, _collider.bounds.extents.y, 0),
                transform.up * -1,
                _jumpQueueSkinWidth,
                _feetMask);

           _jumpQueued = _canBeMoved && (bool)jumpQueueHit && Input.GetButtonDown("Jump");

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
            if(!_dashing)
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

        float intervalPerFrame = -1;
        if (attack.Frames != null && attack.Frames.Length > 0)
        {
            intervalPerFrame = attack.DamageDelay / attack.Frames.Length;
            foreach(Sprite frame in attack.Frames)
            {
                _spriteRenderer.sprite = frame;
                yield return new WaitForSeconds(intervalPerFrame);
            }
        }
        else
        {
            yield return new WaitForSeconds(attack.DamageDelay);
        }
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
    }

    public void ToggleMovementControls()
    {
        _canBeMoved = !_canBeMoved;
    }

    public void ToggleAttackControls()
    {
        _canAttack = !_canAttack;
    }

    public void DoAttack()
    {
        switch(_element)
        {
            case ElementType.DIRT:
                if(_dirtAttacks == null || _dirtAttacks.Length == 0)
                {
                    break;
                }
                _canBeMoved = false;
                _canAttack = false;
                if (_currentAttackCoroutine != null)
                {
                    StopCoroutine(_currentAttackCoroutine);
                }
                _currentAttackCoroutine = StartCoroutine(DoDirtAttack(_dirtAttacks[_currentAttack % _dirtAttacks.Length]));
                _currentAttack++;
                break;
        }
    }
}
