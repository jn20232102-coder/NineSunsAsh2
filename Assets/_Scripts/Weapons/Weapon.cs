using System.Collections.Generic;
using NineSunsAsh.Weapons.Components;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace NineSunsAsh.Weapons
{
    /// <summary>
    /// 挂载在GameObject上的武器实体类。
    /// 现有逻辑：持有数据，并提供 Enter (开始攻击) 和 Exit (结束攻击) 的接口
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [field: SerializeField][Tooltip("武器数据配置")] public WeaponDataSO Data { get; private set; }
        [Tooltip("武器持有者")] public CharacterBase Owner { get; private set;}
        [Tooltip("武器动画控制器，位于SpriteRenderer")] public Animator BaseAnimator { get; private set; }
        
        public SpriteRenderer BaseRenderer { get; private set;}
        
        // 运行时持有的所有逻辑组件
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private List<WeaponComponentBase> _components = new List<WeaponComponentBase>();

        /// <summary>
        /// 用于从外部注入依赖，比如在捡起武器或者游戏开始时调用
        /// </summary>
        /// <param name="owner">武器持有者</param> 
        /// <param name="animator">武器动画控制器，位于SpriteRender</param>
        /// <param name="renderer">默认武器渲染器，位于SpriteRender</param>
        public void SetCore(CharacterBase owner, Animator animator, SpriteRenderer renderer)
        {
            Owner = owner;
            BaseAnimator = animator;
            BaseRenderer = renderer;
        }
        
        public void SetData(WeaponDataSO weaponData)
        {
            Data = weaponData;
            _components.Clear();
        }

        /// <summary> 供 WeaponComponentData 调用，将生成的组件注册到逻辑组件列表中 </summary>
        public void AddComponent(WeaponComponentBase component)
        {
            _components.Add(component);
            component.Init(this);
        }
        
        /// <summary> 进入攻击状态，触发一次攻击 </summary>
        public void Enter()
        {
            Debug.Log($"{transform.name} Enter,开始攻击逻辑");
            
            // 遍历组件调用OnEnter
            foreach (var component in _components)
            {
                component.Enter();
            }
        }

        /// <summary> 结束攻击状态，停止当前攻击 </summary>
        public void Exit()
        {
            // 遍历组件调用OnExit
            foreach (var component in _components)
            {
                component.Exit();
            }
            Debug.Log($"{transform.name} Exit,结束攻击逻辑");
        }
        
        /// <summary> 获取特定类型的组件（供内部交互使用）</summary>
        public T GetWeaponComponent<T>() where T : WeaponComponentBase
        {
            foreach (var comp in _components)
            {
                if (comp is T typedComp)
                {
                    return typedComp;
                }
            }
            return null;
        }
    }
}