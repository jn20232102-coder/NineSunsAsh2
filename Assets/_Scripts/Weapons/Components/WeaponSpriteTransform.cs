using UnityEngine;

namespace NineSunsAsh.Weapons.Components
{
    public class WeaponSpriteTransform : WeaponComponentBase<WeaponSpriteTransformData>
    {
        private Transform _pivotTransform; // 旋转枢轴
        private float _attackStartTime;

        protected override void Start()
        {
            base.Start();
            
            if (weapon.BaseRenderer != null && weapon.BaseRenderer.transform.childCount > 0)
            {
                _pivotTransform = weapon.BaseRenderer.transform.GetChild(0);
                ResetToStartRotation();
            }
        }

        public override void Enter()
        {
            base.Enter();
            _attackStartTime = Time.time;
            ResetToStartRotation();
        }
        
        public override void Exit()
        {
            base.Exit();
            ResetToStartRotation();
        }

        private void Update()
        {
            if (!isAttackActive || _pivotTransform == null) return;

            float timeElapsed = Time.time - _attackStartTime;
            float percentage = timeElapsed / data.swingDuration;

            if (percentage >= 1f)
            {
                FinishAttack();
                return;
            }

            float currentZ = Mathf.Lerp(data.startRotation, data.endRotation, percentage);
            _pivotTransform.localRotation = Quaternion.Euler(0, 0, currentZ);
        }

        private void FinishAttack()
        {
            weapon.Exit(); 
        }

        private void ResetToStartRotation()
        {
            if (_pivotTransform != null)
            {
                _pivotTransform.localRotation = Quaternion.Euler(0, 0, data.startRotation);
            }
        }

        private void OnDisable()
        {
            isAttackActive = false;
        }

        // ================== 修复后的可视化 Gizmos ==================
        private void OnDrawGizmos()
        {
            // 如果没有数据，就不画
            if (data == null) return;

            // 1. 确定绘制的基准坐标系
            // 如果正在运行且有枢轴，以父物体为基准（因为枢轴自己在转）
            // 如果在编辑器模式（_pivotTransform为空），以 WeaponHolder 自身为基准
            Matrix4x4 drawMatrix = Gizmos.matrix; 
            if (_pivotTransform != null && _pivotTransform.parent != null)
            {
                drawMatrix = _pivotTransform.parent.localToWorldMatrix;
            }
            else
            {
                drawMatrix = transform.localToWorldMatrix; 
            }

            Gizmos.matrix = drawMatrix;
            Vector3 center = Vector3.zero;
            float radius = 2.5f;

            // 2. “范围线” (白色)
            Gizmos.color = new Color(1, 1, 1, 0.5f); // 半透明白
            Vector3 startDir = Quaternion.Euler(0, 0, data.startRotation) * Vector3.up;
            Vector3 endDir = Quaternion.Euler(0, 0, data.endRotation) * Vector3.up;

            Gizmos.DrawLine(center, center + startDir * radius); // 起始位置线
            Gizmos.DrawLine(center, center + endDir * radius);   // 结束位置线
            
            // 3. 绘制当前的红线 (只在攻击时或有引用时显示)
            if (_pivotTransform != null)
            {
                // 将矩阵切换为“枢轴自己”的坐标系，这样 Y 轴就是武器的当前朝向
                Gizmos.matrix = _pivotTransform.localToWorldMatrix;
                Gizmos.color = Color.red;
                
                // 画一条代表“剑身方向”的线
                // 警告：这里假设你的剑素材是“头朝上”的。
                // 如果你的素材是横着的，红线就会和剑成90度。
                Gizmos.DrawLine(Vector3.zero, Vector3.up * radius); 
            }
        }
    }
}