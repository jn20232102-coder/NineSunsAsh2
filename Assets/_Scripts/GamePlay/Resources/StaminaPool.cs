using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPool : MonoBehaviour
{
    [Header("Stamina")] 
    [SerializeField] private float max = 100f;
    [SerializeField][Tooltip("每秒回复速度")] private float regenPerSecond = 10f;
    [SerializeField][Tooltip("消耗后多久开始回复")] private float regenDelay = 0.5f;

    public float Max => max;
    public float Current { get; private set; }

    private float _lastSpendTime;
    public event Action<float, float> OnStaminaChanged;

    void Awake()
    {
        Current = max;
    }

    void Update()
    {
        // 体力回复
        if (Time.time >= _lastSpendTime + regenDelay && Current < max)
        {
            Current = Mathf.Min(max, Current + regenPerSecond * Time.deltaTime);
            OnStaminaChanged?.Invoke(Current, max);
        }
    }

    public bool TrySpend(float amount)
    {
        if (amount <= 0f) return true;
        if (Current < amount) return false;

        Current -= amount;
        _lastSpendTime = Time.time;
        OnStaminaChanged?.Invoke(Current, max);
        
        return true;
    }

    public void Add(float amount)
    {
        if (amount <= 0f) return;
        Current = Mathf.Min(max, Current + amount);
    }

    public void SetMax(float newMax, bool refill = false)
    {
        max = newMax;
        if (refill) Current = max;
        OnStaminaChanged?.Invoke(Current, max);
    }
}
