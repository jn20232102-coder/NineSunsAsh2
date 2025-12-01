using System;
using UnityEngine;

namespace NineSunsAsh.Weapons.Components
{
    /// <summary>
    /// 所有运行时逻辑组件的基类 (Logic)
    /// </summary>
    /// <typeparam name="T">组件对应的数据类型 (Data)，泛型约束确保类型安全</typeparam>
    public abstract class WeaponComponentBase<T> : WeaponComponentBase where T : WeaponComponentData
    {
        protected T data;

        protected override void Awake()
        {
            base.Awake();

            data = weapon.Data.GetData<T>();
        }
    }
    
    /// <summary>
    /// 非泛型的抽象基类，用于 Weapon 类统一管理 “List<WeaponComponent/>"
    /// </summary>
    public abstract class WeaponComponentBase : MonoBehaviour
    {
        protected Weapon weapon;


        [Tooltip("核心动画状态机判定条件")] protected bool isAttackActive;

        public virtual void Init(Weapon inWeapon)
        {
            this.weapon = inWeapon;
        }

        protected virtual void Awake()
        {
            weapon = GetComponent<Weapon>();
        }

        protected virtual void Start()
        {
            var core = weapon.Owner;
        }

        /// <summary>
        /// 设置isAttackActive为true
        /// </summary>
        public virtual void Enter()
        {
            isAttackActive = true;
        }

        /// <summary>
        /// 设置isAttackActive为false
        /// </summary>
        public virtual void Exit()
        {
            isAttackActive = false;
        }
        
        protected virtual void OnDestroy()
        {
            
        }
    }
}