using UnityEngine;

namespace NineSunsAsh.Weapons.Components
{
    [System.Serializable]
    public class WeaponSpriteTransformData : WeaponComponentData
    {
        [Header("程序化动画参数")] 
        public float startRotation = 0f;
        public float endRotation = -135f;
        public float swingDuration = 0.2f;
        
        [Header("Pivot Settings")]
        [Tooltip("视觉偏移：通过调整这个值，让剑柄对准角色的手")]
        public Vector3 visualOffset = new Vector3(0, 0.5f, 0); // 默认向上偏移一点
        
        public override void InitializeAttackData(Weapon weapon)
        {
            var component = weapon.gameObject.AddComponent<WeaponSpriteTransform>();
            component.Init(weapon);
            weapon.AddComponent(component);
        }
    }
}