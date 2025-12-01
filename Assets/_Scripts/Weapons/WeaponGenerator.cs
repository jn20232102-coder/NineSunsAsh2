using System;
using UnityEngine;

namespace NineSunsAsh.Weapons
{
    [RequireComponent(typeof(Weapon))]
    public class WeaponGenerator : MonoBehaviour
    {
        private Weapon _weapon;
        [SerializeField][Tooltip("拖入想测试的武器数据")] private WeaponDataSO data;

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
        }

        private void Start()
        {
            // ================== 新增：自动初始化依赖 ==================
            // 1. 尝试在当前物体或父物体上找 CharacterBase
            var owner = GetComponentInParent<CharacterBase>();
            
            // 2. 尝试找 Animator (通常在 Model 子物体上，或者 Player 根物体上)
            // 我们先往父级找，再从父级往下找所有子物体，确保能找到兄弟节点 Model 里的 Animator
            Animator animator = GetComponentInParent<Animator>();
            if (animator == null && transform.parent != null)
            {
                animator = transform.parent.GetComponentInChildren<Animator>();
            }
            
            // 如果还是没找到，报警
            if (animator == null)
            {
                Debug.LogError($"{name} 找不到 Animator！请确保 Player 下面有挂 Animator 组件。");
            }
            
            // 3. 查找SpriteRenderer
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr == null && transform.parent != null)
            {
                // 尝试去兄弟节点找 (假设结构是 Player -> [Model, WeaponHolder])
                sr = transform.parent.GetComponentInChildren<SpriteRenderer>();
            }
            if (sr == null) Debug.LogError("找不到 Player 的 SpriteRenderer！");

            // 4. 注入依赖 (这一步是修复 NullReferenceException 的关键)
            _weapon.SetCore(owner, animator, sr);

            // 5. 一切就绪，生成武器
            GenerateWeapon(data);
        }

        public void GenerateWeapon(WeaponDataSO weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogError("没有配置WeaponDataSO，生成武器失败");
                return;
            }

            _weapon.SetData(weaponData);
            
            // 1. TODO：若更换武器，清理旧组件
            
            // 2. 遍历数据，添加组件
            foreach (var componentData in weaponData.componentData)
            {
                if (componentData == null) continue;
                
                // 调用 WeaponComponentData 里的方法，决定添加哪个 Component
                componentData.InitializeAttackData(_weapon);
            }
        }
    }
}