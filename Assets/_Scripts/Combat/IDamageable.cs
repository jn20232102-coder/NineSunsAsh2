using System;
using System.Collections;
using System.Collections.Generic;
using NineSunsAsh.Combat;
using UnityEngine;

namespace NineSunsAsh.Combat
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        bool IsInvulnerable { get; }
    
        // 受伤入口，外部调用此函数，不直接修改血量
        void ReceiveHit(in HitStructure hit);

        event Action<float, float> OnHealthChanged; // (currentHealth, maxHealth)
        event Action<HitStructure> OnDamaged;
        event Action OnDeath;
    }
}

