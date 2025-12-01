using NineSunsAsh.Combat;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    protected override void Awake()
    {
        base.Awake();
        OnDamaged += HandleDamaged;
    }

    private void HandleDamaged(HitStructure hit)
    {
        Debug.Log($"[EnemyCharacter] {name} 被打到了，伤害={hit.amount}, 类型={hit.type}");
    }
    
    protected override void OnDeathInternal()
    {
        base.OnDeathInternal();
        // TODO: 掉落 Loot / 播放死亡动画 / 回收到对象池
    }
    
    
}
