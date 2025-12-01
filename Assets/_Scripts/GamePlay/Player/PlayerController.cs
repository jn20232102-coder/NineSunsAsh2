using System.Collections;
using System.Collections.Generic;
using NineSunsAsh.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("引用")]
    private Player_Controls _playerControls;
    private PlayerCharacter _character;
    private Rigidbody _rb;
    private PlayerInteractor _interactor; // 角色交互
    private Weapon _weapon;
    
    
    private DashAbility _dash;
    private SprintAbility _sprint;
    
    private Vector2 _moveInput; // movement
    private Vector3 _lastMoveDir = Vector3.forward; 

    private void Awake()
    {
        _playerControls = new Player_Controls();
        _rb = GetComponent<Rigidbody>();
        _character = GetComponent<PlayerCharacter>();
        _interactor = GetComponentInChildren<PlayerInteractor>();
        _dash = GetComponent<DashAbility>();
        _sprint = GetComponent<SprintAbility>();
        _weapon = GetComponentInChildren<Weapon>();
        if (_weapon == null) Debug.LogWarning("PlayerController: 找不到Weapon组件");
    }

    private void OnEnable()
    {
        _playerControls.Enable();

        _playerControls.Movement.Interact.performed += OnInteract;
        _playerControls.Movement.Dash.performed += OnDash;
        _playerControls.Movement.Sprint.started += OnSprintStarted; // 按下shift
        _playerControls.Movement.Sprint.canceled += OnSprintCanceled;  // 松开 Shift

        _playerControls.Combat.Attack.started += OnAttackStarted;
        _playerControls.Combat.Attack.canceled += OnAttackCanceled;
    }

    private void OnDisable()
    {
        _playerControls.Movement.Dash.performed -= OnDash;
        _playerControls.Movement.Interact.performed -= OnInteract;
        _playerControls.Movement.Sprint.started  -= OnSprintStarted;
        _playerControls.Movement.Sprint.canceled -= OnSprintCanceled;

        _playerControls.Combat.Attack.started -= OnAttackStarted;
        _playerControls.Combat.Attack.canceled -= OnAttackCanceled;
        
        _playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        // 冲刺/翻滚
        if (_dash && _dash.IsDashing)
        {
            _dash.Step(Time.fixedDeltaTime);
            return;
        }
        
        // 持续体力消耗
        _sprint?.Tick(Time.fixedDeltaTime);
        
        Move();
    }

    private void PlayerInput()
    {
        _moveInput = _playerControls.Movement.Move.ReadValue<Vector2>();
        _dash?.SetSteerInput(_moveInput); // 冲刺中接受输入进行轻微的方向改变
    }

    private void Move()
    {
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
        if (move.sqrMagnitude > 0.0001f) 
            _lastMoveDir = move.normalized;

        float speedMul = _sprint ? _sprint.CurrentSpeedMultiplier : 1f;
        _rb.MovePosition(_rb.position + move.normalized * (_character.config.moveSpeed* speedMul * Time.fixedDeltaTime));
    }
    
    private void OnDash(InputAction.CallbackContext obj)
    {
        Vector3 dir = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        if (dir.sqrMagnitude < 0.0001f)
            dir = _lastMoveDir;
        _dash?.TryDash(dir);
    }
    
    private void OnSprintStarted(InputAction.CallbackContext obj)
    {
        _sprint?.TryStart();
    }
    private void OnSprintCanceled(InputAction.CallbackContext obj)
    {
        _sprint?.Stop();
    }
    
    /// <summary> 角色交互 </summary>
    private void OnInteract(InputAction.CallbackContext _)
    {
        _interactor?.TryInteract();
    }
    
    
    private void OnAttackStarted(InputAction.CallbackContext obj)
    {
        _weapon?.Enter();
    }
    
    private void OnAttackCanceled(InputAction.CallbackContext obj)
    {
        
    }
}
