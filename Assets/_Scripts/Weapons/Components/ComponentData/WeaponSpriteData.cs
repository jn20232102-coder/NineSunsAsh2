using UnityEngine;

namespace NineSunsAsh.Weapons.Components
{
    /// <summary>
    /// 武器spriteRenderer，武器外观数据
    /// </summary>
    [System.Serializable]
    public class WeaponSpriteData : WeaponComponentData
    {
        [Tooltip("武器基础贴图")] public Sprite weaponSprite;
        
        [Header("Visual Settings")]
        [Tooltip("武器的视觉预制体 (包含SpriteRenderer, 粒子等)")]
        public GameObject weaponVisualPrefab;
        
        public override void InitializeAttackData(Weapon weapon)
        {
            var component = weapon.gameObject.AddComponent<WeaponSprite>();
            weapon.AddComponent(component);
        }
    }
}