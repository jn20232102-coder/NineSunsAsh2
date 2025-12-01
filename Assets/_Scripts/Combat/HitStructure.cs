using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NineSunsAsh.Combat
{
    [Serializable]
    public struct HitStructure
    {
        [Header("基础数值")]
        [Tooltip("原始伤害值/攻击力，未应用计算")] public float amount; // 原始伤害
        [Tooltip("伤害类型")] public DamageType type; // 伤害类型
        
        [Header("物理与来源")]
        [Tooltip("攻击发起的对象，\n处理仇恨、经验值、反伤等逻辑")]public GameObject instigator; // 攻击发起者
        [Tooltip("攻击发起的物理位置。\n处理击退方向和格挡朝向判定。")] public Vector3 sourcePosition; 
        [Tooltip("击退力度")] public float knockbackForce; // 击退力度
        [Tooltip("攻击命中点坐标")] public Vector3? point; // 命中点（可选）
        
        
        [Header("特殊机制")]
        [Tooltip("破防层数/削韧能力")]
        public int blockBreakingPower;
        [Tooltip("本次攻击是否【不可格挡】\nTrue = 穿透防御，如地刺、毒气; False = 可被格挡/招架]")]
        public bool isUnblockable;
        [Tooltip("本次攻击是否【不可招架】。\nTrue = 完美格挡无法触发招架效果; False = 可招架")]
        public bool isUnparryable;
        [Tooltip("是否忽略受击者的无敌帧（iFrame）")]
        public bool ignoreInvulnerability;
        
        /// <summary>
        /// 快速构造函数
        /// </summary>
        /// <param name="amount">伤害值</param>
        /// <param name="sourcePos">攻击来源点</param>
        /// <param name="knockback">击退力</param>
        /// <param name="instigator">攻击者</param>
        public HitStructure(float amount, Vector3 sourcePos, float knockback, GameObject instigator)
        {
            this.amount = amount;
            sourcePosition = sourcePos;
            knockbackForce = knockback;
            this.instigator = instigator;
            
            // 默认值：普通物理伤害，可格挡，可招架，消耗1层防御块
            type = DamageType.Physical;
            blockBreakingPower = 1;
            isUnblockable = false;
            isUnparryable = false;
            ignoreInvulnerability = false;
            point = null;
        }
    }
}

