using System.Collections.Generic;
using UnityEngine;

namespace NineSunsAsh.Weapons
{
    [CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data/Basic Weapon", order = 0)]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("基础信息")] 
        public string weaponName; // 武器名称
        public Sprite icon; // UI图标

        [Header("组件数据列表")] 
        [SerializeReference]
        public List<WeaponComponentData> componentData = new List<WeaponComponentData>(); // 放所有构成这把武器的组件数据（伤害、碰撞框等）

        public T GetData<T>()
        {
            foreach (var item in componentData)
            {
                if (item.GetType() == typeof(T))
                {
                    return (T)(object)item; // 两次类型转换（先object），绕过编译器对从具体类型直接转为泛型 T 的限制
                }
            }
            return default;
        }
    }
}