using System;
using UnityEngine;

namespace NineSunsAsh.Weapons.Components
{
    public class WeaponSprite : WeaponComponentBase<WeaponSpriteData>
    {
        private SpriteRenderer _weaponRenderer;
        private Transform _baseSpriteTransform;
        
        private GameObject _currentVisualInstance;

        protected override void Start()
        {
            base.Start();
            
            // 必须有渲染器引用才能继续（SetCore必须成功）
            if (weapon.BaseRenderer == null) return;
            
            _baseSpriteTransform = weapon.BaseRenderer.transform;
            
            // 优先级 1: 如果配置了预制体，使用预制体
            if (data.weaponVisualPrefab != null)
            {
                _currentVisualInstance = Instantiate(data.weaponVisualPrefab, _baseSpriteTransform, false);
            }
            // 优先级 2: 如果没有预制体但有贴图，自动生成一个
            else if (data.weaponSprite != null)
            {
                _currentVisualInstance = new GameObject($"{weapon.name}_GeneratedVisual");
                _currentVisualInstance.transform.SetParent(_baseSpriteTransform, false);
                
                var sr = _currentVisualInstance.AddComponent<SpriteRenderer>();
                sr.sprite = data.weaponSprite;
            }
            
            // 通用设置：如果有生成的物体，进行位置和层级初始化
            if (_currentVisualInstance != null)
            {
                _currentVisualInstance.transform.localPosition = Vector3.zero;
                _currentVisualInstance.transform.localRotation = Quaternion.identity;

                // 确保武器显示在角色上层
                var weaponSr = _currentVisualInstance.GetComponentInChildren<SpriteRenderer>();
                if (weaponSr != null)
                {
                    weaponSr.sortingOrder = weapon.BaseRenderer.sortingOrder + 1;
                }
                
                // 缓存引用以便销毁
                _weaponRenderer = weaponSr; 
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // 武器销毁时，把生成的视觉物体也删掉，否则会残留
            if (_weaponRenderer != null)
            {
                Destroy(_weaponRenderer.gameObject);
            }
        }
    }
}