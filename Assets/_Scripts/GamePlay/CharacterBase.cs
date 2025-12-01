using System;
using System.Collections;
using System.Collections.Generic;
using NineSunsAsh.Combat;
using UnityEngine;

/// <summary>
/// 角色基类：实现IDamageable + 统一无敌帧管理 + 基础生命/死亡逻辑
/// </summary>
[DisallowMultipleComponent]
public class CharacterBase : MonoBehaviour, IDamageable
{
    [Header("Config")] 
    public CharacterStatsConfig config;
    
    // 运行时属性
    public float MaxHealth { get; private set; }
    public float Health { get; private set; } 
    public bool IsAlive { get; private set; }
    
    // IFrame: 来源 -> 截止时间
    private readonly Dictionary<InvulnerabilityFrameSource, float> _iFrameUntil = new();

    public bool IsInvulnerable
    {
        get
        {
            float now = Time.time;
            foreach (var kv in _iFrameUntil)
                if (kv.Value > now)
                    return true;
            return false;
        }
    }
    
    // 事件
    public event Action<float, float> OnHealthChanged;
    public event Action<HitStructure> OnDamaged;
    public event Action OnDeath;
    public event Action<bool> OnInvulnerabilityChanged; // 可给特效/UI用

    protected virtual void Awake()
    {
        MaxHealth = config ? Mathf.Max(1f, config.maxHealth) : 100f;
        Health = MaxHealth;
        IsAlive = true;
    }
    
    #region I-Frame 统一入口
    public void BeginIFrame(float duration, InvulnerabilityFrameSource src)
    {
        float until = Time.time + Mathf.Max(0f, duration);
        if (_iFrameUntil.TryGetValue(src, out var old))
        {
            _iFrameUntil[src] = Mathf.Max(old, until);
        }
        else
        {
            _iFrameUntil.Add(src, until);
        }
        
        OnInvulnerabilityChanged?.Invoke(true);
    }

    public void EndIFrame(InvulnerabilityFrameSource src)
    {
        if (_iFrameUntil.Remove(src))
            OnInvulnerabilityChanged?.Invoke(false);
    }
    
    public void ClearAllIFrames()
    {
        if (_iFrameUntil.Count > 0)
        {
            _iFrameUntil.Clear();
            OnInvulnerabilityChanged?.Invoke(false);
        }
    }
    #endregion

    #region Damage
    public virtual void ReceiveHit(in HitStructure hit)
    {
        if (!IsAlive) return;
        if (!hit.ignoreInvulnerability && IsInvulnerable) return;
        
        float final = ApplyResistance(hit.amount, hit.type);
        if (final <= 0f) return;
        
        Health = Mathf.Max(0f, Health - final);
        OnDamaged?.Invoke(hit);
        OnHealthChanged?.Invoke(Health, MaxHealth);

        // 受击无敌帧
        if (config && config.hurtIFrame > 0f)
            BeginIFrame(config.hurtIFrame, InvulnerabilityFrameSource.Hurt);

        // 死亡触发
        if (Health <= 0f)
        {
            IsAlive = false;
            OnDeath?.Invoke();
            OnDeathInternal();
        }
        
    }
    protected virtual float ApplyResistance(float raw, DamageType type)
    {
        if (!config) return raw;
        float resist = Mathf.Clamp01(config.GetResist(type));
        return Mathf.Max(0f, raw * (1f - resist));
    }
    
    /// <summary> 死亡后通用处理：默认禁用碰撞和输入，允许子类重写 /// </summary>
    protected virtual void OnDeathInternal()
    {
        // 关闭碰撞、关闭刚体、禁用控制器、等待动画/回放等
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        
        // TODO: 通知掉落物/计分/RunFlow等由子类实现
    }

    #endregion
}
