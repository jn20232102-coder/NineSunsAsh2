using System.Collections;
using System.Collections.Generic;
using NineSunsAsh.Combat; // 引用你的战斗系统命名空间
using UnityEngine;

namespace NineSunsAsh.Weapons
{
    /// <summary>
    /// 挂载在刀光特效预制体上。
    /// 负责：播放动画(自动) -> 判定伤害 -> 播放完销毁
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))] // 触发器通常需要一个刚体(设为IsKinematic)
    public class MeleeSlashProjectile : MonoBehaviour
    {
        [Header("Debug Info")]
        [SerializeField] private float damage;
        [SerializeField] private float knockbackForce;
        private GameObject instigator; // 攻击者
        
        // 防止同一刀对同一个敌人造成多次伤害 (针对多段判定的情况，单次攻击可保留)
        private HashSet<GameObject> _hitTargets = new HashSet<GameObject>();

        // 供外部(WeaponSlashSpawner)调用的初始化方法
        public void Initialize(float damageAmount, float knockback, GameObject attacker, float lifetime = 0.5f)
        {
            this.damage = damageAmount;
            this.knockbackForce = knockback;
            this.instigator = attacker;

            // 1. 销毁策略：如果有动画，按动画长度销毁；如果没有，按固定时间销毁
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null)
            {
                // 获取当前动画片段的长度
                float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
                // 稍微多给一点缓冲时间(0.1s)，防止动画还没播完就没了
                Destroy(gameObject, animLength); 
            }
            else
            {
                Destroy(gameObject, lifetime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 1. 排除自己
            if (other.gameObject == instigator) return;
            
            // 2. 排除已经打过的目标（防止一帧内多次触发，或者穿过身体时多次触发）
            if (_hitTargets.Contains(other.gameObject)) return;

            // 3. 尝试造成伤害
            if (other.TryGetComponent(out IDamageable target))
            {
                // 记录已命中
                _hitTargets.Add(other.gameObject);

                // 组装伤害信息
                HitStructure hit = new HitStructure(
                    amount: damage,
                    sourcePos: transform.position, // 刀光的位置作为击退来源
                    knockback: knockbackForce,
                    instigator: instigator
                );

                // 执行伤害
                target.ReceiveHit(hit);
                
                // 可选：在这里生成“击中反馈”特效（比如飙血、火花）
                // Instantiate(hitVFX, other.ClosestPoint(transform.position), Quaternion.identity);
                Debug.Log($"刀光击中了: {other.name}");
            }
        }
    }
}