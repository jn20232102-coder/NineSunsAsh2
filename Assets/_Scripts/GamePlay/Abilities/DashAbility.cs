using System;
using System.Collections;
using System.Collections.Generic;
using NineSunsAsh.Combat;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DashAbility : MonoBehaviour
{
    [Header("Dash Params")] 
    [SerializeField] private float distance = 3f;
    [SerializeField] private float duration = 0.18f;
    [SerializeField] private float cooldown = 0.5f;
    
    [Header("Steer")]
    [Range(0f, 1f)][SerializeField] private float steerPercent = 0.3f; // 冲刺过程中允许输入影响方向的比例（方向盘的最大角度）
    [SerializeField] private float steerLerp = 0.15f; // 每帧插值强度（打方向盘的速度）

    [Header("Invulnerability")]
    [SerializeField] private float dashIFrame = 0.12f; // 冲刺无敌时长
    
    [Header("Stamina")]
    [SerializeField] private float staminaCost = 20f;
    
    public bool IsDashing { get; private set; }
    public event Action OnDashStarted, OnDashEnded;

    private Rigidbody _rb;
    CharacterBase _character;
    StaminaPool _stamina;

    private Vector3 _dashDir;
    private Vector2 _steerInput;
    private float _elapsed; // 记录已经冲刺时间
    private float _speed, _lastDashTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _character = GetComponent<CharacterBase>();
        _stamina = GetComponent<StaminaPool>();
        
        // 刚体设置
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    
    /// <summary>PlayerController 每帧把 Move 输入传给我，用于冲刺中的轻微转向。</summary>
    public void SetSteerInput(Vector2 move) => _steerInput = move;

    /// <summary> 尝试开始冲刺。dir 长度为0时使用角色朝向 </summary>
    public bool TryDash(Vector3 dir)
    {
        if (IsDashing) return false;
        if (Time.time < _lastDashTime + cooldown) return false;
        if (_stamina && !_stamina.TrySpend(staminaCost)) return false;
        
        _dashDir = dir.sqrMagnitude > 0.0001f ? dir.normalized : transform.forward;
        _speed = distance / Mathf.Max(0.01f, duration);

        IsDashing = true;
        _elapsed = 0f;
        _lastDashTime = Time.time;
        
        if (dashIFrame > 0f) 
            _character?.BeginIFrame(dashIFrame, InvulnerabilityFrameSource.Dash);
        OnDashStarted?.Invoke();
        return true;
    }

    /// <summary>在 FixedUpdate 里被控制器调用以推进冲刺。</summary>
    public void Step(float dt)
    {
        if (!IsDashing) return;
        
        // 允许轻微改向
        Vector3 steer = new Vector3(_steerInput.x, 0f, _steerInput.y);
        if (steer.sqrMagnitude > 0.0001f && steerPercent > 0f)
        {
            Vector3 target = (_dashDir + steer.normalized * steerPercent).normalized;
            float t = 1f - Mathf.Exp(-steerLerp * dt);
            _dashDir = Vector3.Slerp(_dashDir, target, t);
        }

        _rb.MovePosition(_rb.position + _dashDir * _speed * dt);

        _elapsed += dt;
        if (_elapsed >= duration)
        {
            IsDashing = false;
            _character?.EndIFrame(InvulnerabilityFrameSource.Dash);
            OnDashEnded?.Invoke();
        }
    }
}
