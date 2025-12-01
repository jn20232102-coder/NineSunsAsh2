using UnityEngine;

namespace NineSunsAsh.Weapons
{
    /// <summary>
    /// 所有武器组件（伤害、判定框、特效）的数据基类
    /// </summary>
    [System.Serializable] 
    public abstract class WeaponComponentData 
    {
        [SerializeField, HideInInspector] private string name;
        
        // 构造函数，设置组件的默认名称
        protected WeaponComponentData()
        {
            name = GetType().Name; // 将当前类型的名称作为组件的名字
        }
        
        /// <summary>
        /// 抽象方法，强制子类实现“如何将自己（武器数据）添加到武器上”
        /// </summary>
        /// <param name="weapon"></param>
        public abstract void InitializeAttackData(Weapon weapon);
    }
}