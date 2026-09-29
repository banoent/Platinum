using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CPlayer : MonoBehaviour
{
    #region 인스펙터
    [Header("이동 / 회전 / 점프")]
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _rotateSpeed = 120.0f;
    [SerializeField] private float _jumpForce = 8.0f;
    [SerializeField] private float _rotateSharpness = 12.0f;
    [SerializeField] private float _runSpeed = 8.0f;


    [Header("땅")]
    [SerializeField] private LayerMask _groundLayer = default;
    [SerializeField] private float _groundRayLength = 1.3f;

    [Header("애니메이션")]
    [SerializeField] private Animator _anim;

    [Header("체력")]
    [SerializeField] private float _maxHp = 100f;
    
    #endregion


    #region 내부변수
    private Rigidbody rb;
    private bool _isGround;
    private bool _isJump;
    private bool _isRun;
    private Transform _cameraTr;
    private Vector3 moveDir = Vector3.zero;
    private float _currentHp;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        if(_cameraTr == null && Camera.main != null)
        {
            _cameraTr = Camera.main.transform;
        }

        _currentHp = _maxHp;
    }
    void Start()
    {
        
    }

    void Update()
    {
        InputMove();
        CheckGround();       
    }

    private void FixedUpdate()
    {
        Move();
        TryJump();
    }


    public void Heal(float value)
    {
        _currentHp += value;

        if (_currentHp > _maxHp)
        {
            _currentHp = _maxHp;
        }
           
    }

    private void InputMove()
    {
        float RightLeftMove = Input.GetAxis("Horizontal");
        float UpDownmove = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(RightLeftMove, 0, UpDownmove);
        input = Vector3.ClampMagnitude(input.normalized, 1.0f);

        _isRun = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        moveDir = (input.sqrMagnitude > 0.0001f) ? BuildMoveDirection(input) : Vector3.zero;
          
        TickRotate(moveDir);        

        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        {
            _isJump = true;
        }

    }

    private void Move()
    {
        float speed = _isRun ? _runSpeed : _moveSpeed;               

        Vector3 velocity = rb.velocity;

        if (_isRun && velocity.sqrMagnitude > 0.0001f)
        {
            _anim.SetBool("bRun", true);
            _anim.SetBool("bWalk", false);
        }
        else if (!_isRun && velocity.sqrMagnitude > 0.0001f)
        {
            _anim.SetBool("bRun", false);
            _anim.SetBool("bWalk", true);
        }
        else
        {
            _anim.SetBool("bRun", false);
            _anim.SetBool("bWalk", false);
        }

        velocity.x =  moveDir.x * speed;
        velocity.z = moveDir.z * speed;

        rb.velocity = velocity;
    }

    private void TryJump()
    {
        if(rb == null)
        {
            return;
        }

        if (!_isJump)
        {
            return;
        }

        _isJump = false;

        if (!_isGround)
        {
            return;
        }


                     
        Vector3 velocity = rb.velocity;
        velocity.y = _jumpForce;
        rb.velocity = velocity;

        _isGround = false;
        _anim.SetTrigger("tJump");               
    }

   

    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        Vector3 dir = Vector3.down;

        int mask = (_groundLayer.value == 0) ? Physics.AllLayers : _groundLayer.value;      

        _isGround = Physics.Raycast(origin, dir, _groundRayLength, mask);
        if (!_isGround)
        {
            _anim.SetBool("bJumping", true);
            _anim.ResetTrigger("tJump");
        }
        else
        {
            _anim.SetBool("bJumping", false);
        }

            Debug.DrawRay(origin, dir * _groundRayLength, _isGround ? Color.green : Color.red);

       

    }

    private Vector3 BuildMoveDirection(Vector3 input)
    {

        if (_cameraTr == null)
        {
            return input.normalized; 
        }

        Vector3 camF = Vector3.ProjectOnPlane(_cameraTr.forward, Vector3.up).normalized;
        Vector3 camR = Vector3.ProjectOnPlane(_cameraTr.right, Vector3.up).normalized;

        Vector3 dir = camF * input.z + camR * input.x;        

        return dir.normalized;
    }

    private void TickRotate(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        // 현재 바라보는 방향에서 목표 방향으로 부드럽게 회전
        Quaternion targerRot = Quaternion.LookRotation(moveDir, Vector3.up);

        transform.rotation = Quaternion.Slerp
            (
                transform.rotation,
                targerRot,
                1.0f - Mathf.Exp(-_rotateSharpness * Time.deltaTime)

            );


    }
}
