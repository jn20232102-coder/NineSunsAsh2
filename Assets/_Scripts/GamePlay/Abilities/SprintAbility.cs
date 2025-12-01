using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintAbility : MonoBehaviour
{
    [Header("Sprint")]
    [SerializeField] float speedMultiplier = 1.6f; // 奔跑速度倍率
    [SerializeField] float staminaPerSecond = 15f; // 每秒消耗体力
    [SerializeField] float minStaminaToStart = 7.5f; // 起跑所需最小体力
    
    public bool IsSprinting { get; private set; }
    public float CurrentSpeedMultiplier => IsSprinting ? speedMultiplier : 1f;
    
    public event Action OnSprintStarted, OnSprintEnded;
    
    StaminaPool _stamina;

    void Awake()
    {
        _stamina = GetComponent<StaminaPool>();
    }

    public bool TryStart()
    {
        if (IsSprinting) return false;
        if (_stamina && _stamina.Current < minStaminaToStart) return false;
        
        IsSprinting = true;
        OnSprintStarted?.Invoke();
        return true;
    }

    public void Stop()
    {
        if (!IsSprinting) return;
        IsSprinting = false;
        OnSprintEnded?.Invoke();
    }

    /// <summary>每帧/每物理步消耗体力；不足则自动停止。</summary>
    public void Tick(float dt)
    {
        if (!IsSprinting) return;
        if (_stamina == null) return;

        float cost = staminaPerSecond * Mathf.Max(0f, dt);
        if (!_stamina.TrySpend(cost))
            Stop();
    }
 }
