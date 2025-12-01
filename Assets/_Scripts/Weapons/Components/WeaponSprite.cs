using UnityEngine;

namespace NineSunsAsh.Weapons.Components
{
    public class WeaponSprite : WeaponComponentBase<WeaponSpriteData>
    {
        private GameObject _currentVisualInstance;

        protected override void Start()
        {
            base.Start();
            
            // 如果核心引用缺失或数据没填，直接罢工
            if (weapon.BaseRenderer == null || data.weaponVisualPrefab == null) 
            {
                Debug.LogWarning($"{weapon.name} 的 WeaponSprite 组件无法初始化！缺少 Renderer 或 Prefab。");
                return;
            }

            // 1. 实例化：直接生成在 Model (BaseRenderer) 下面
            _currentVisualInstance = Instantiate(data.weaponVisualPrefab, weapon.BaseRenderer.transform, false);

            // 2. 归零：确保它相对于 Model 是归零的（位置完全由 Prefab 内部决定）
            _currentVisualInstance.transform.localPosition = Vector3.zero;
            _currentVisualInstance.transform.localRotation = Quaternion.identity;
            _currentVisualInstance.transform.localScale = Vector3.one;

            // 3. 层级管理：确保武器显示在角色前面
            var weaponSr = _currentVisualInstance.GetComponentInChildren<SpriteRenderer>();
            if (weaponSr != null)
            {
                weaponSr.sortingOrder = weapon.BaseRenderer.sortingOrder + 1;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 换武器时，销毁旧的 Visual 实例
            if (_currentVisualInstance != null)
            {
                Destroy(_currentVisualInstance);
            }
        }
    }
}