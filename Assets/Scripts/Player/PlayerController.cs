using UnityEngine;

[RequireComponent (typeof(CapsuleCollider2D))]
[RequireComponent (typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb2d;
    private Collider2D _collider;

    [SerializeField]
    private float _moveSpeed = 1.0f;

    [SerializeField]
    private float _dirtMoveSpeed = 1.0f;

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

    private bool _airborne = false;
    private bool _jumpQueued = false;
    private bool _hasDoubleJump = false;
    private bool _dashing = false;
    private bool _dashedOnce = false;
    private float _dashDelta = 0.0f;
    private float _dashSign = 1;

    [SerializeField]
    private ElementType element = ElementType.NONE;
    
    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _rb2d.linearVelocityX = Input.GetAxis("Horizontal") * 
            (element == ElementType.DIRT ? _dirtMoveSpeed : _moveSpeed);


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
            if(element != ElementType.DIRT && (Input.GetButtonDown("Jump") || _jumpQueued))
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

           _jumpQueued = (bool)jumpQueueHit && Input.GetButtonDown("Jump");

            if (element == ElementType.AIR)
            {
                if (_hasDoubleJump && Input.GetButtonDown("Jump"))
                {
                    _rb2d.linearVelocityY = _doubleJumpSpeed;
                    _hasDoubleJump = false;
                    _jumpQueued = false;
                }
                if(!_dashedOnce && Input.GetKeyDown(KeyCode.LeftShift))
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
}
