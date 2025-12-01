using UnityEngine;

namespace NineSunsAsh.Weapons.Components
{
    [System.Serializable]
    public class WeaponSpriteData : WeaponComponentData
    {
        [Header("Visual Prefab (必填)")]
        [Tooltip("武器的外观预制体。实例化后会挂载到 Player/Model 下。")]
        public GameObject weaponVisualPrefab; 
        
        public override void InitializeAttackData(Weapon weapon)
        {
            // 确保添加了 WeaponSprite 组件来处理这个 Prefab
            var component = weapon.gameObject.AddComponent<WeaponSprite>();
            weapon.AddComponent(component);
        }
    }
}