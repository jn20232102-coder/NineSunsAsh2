using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionIndicator3D : MonoBehaviour
{
    [Header("Refs")] 
    [SerializeField] private Camera cam;
    [SerializeField] private Transform owner;
    
    [Header("Orbit around owner pivot")]
    [SerializeField][Tooltip("是否绕pivot旋转")] bool orbitAroundOwner = true; // ← 开关：是否绕pivot旋转
    [SerializeField][Tooltip("半径：离pivot多远")] float radius = 0.8f; // ← 半径：离pivot多远
    [SerializeField][Tooltip("高度：离地/离pivot的Y偏移")] float height = 0.02f; // ← 高度：离地/离pivot的Y偏移
    [SerializeField, Tooltip("菱形尖头与模型本地方向的偏差(度)。尖头沿本地+Y=0；+X=90；-X=-90；-Y=180")]
    float yawOffsetInPlane = 0f;
    [SerializeField, Tooltip("让尖头落在半径上，而不是菱形中心。按世界单位调整")]
    float tipOffset = 0f;

    [Header("Targeting")] 
    [SerializeField] private bool useGroundRaycast = true;
    [SerializeField] private LayerMask groundMask = -1;
    [SerializeField] private float planeY = 0f; // 水平面高度（不使用Raycast时使用）

    [Header("Debug")] 
    [Tooltip("命中点")] public Vector3 targetWorld;
    [Tooltip("水平朝向")] public Vector3 aimDir;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!owner) owner = transform.parent ? transform.parent : transform;
    }

    void Update()
    {
        if (!cam || !owner) return;
        
        // 1. 鼠标屏幕坐标
        Vector2 mousePos;
        #if ENABLE_INPUT_SYSTEM
        mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
        #else
        mousePos = Input.mousePosition;
        #endif
        
        // 2. 屏幕射线
        Ray ray = cam.ScreenPointToRay(mousePos);
        
        // 3. 地面命中点
        if (useGroundRaycast)
        {
            if (Physics.Raycast(ray, out var hit, 500f, groundMask, QueryTriggerInteraction.Ignore))
            {
                targetWorld = hit.point;
            }
            else
            {
                return;
            }
        }
        else
        {
            // 与水平面 y = PlaneY 相交
            var plane = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));
            if (plane.Raycast(ray, out float dist))
                targetWorld = ray.GetPoint(dist);
            else
                return; // 射线与平面平行
        }
        
        // 4. 只在水平面上取方向（绕Y轴朝向）
        Vector3 from = owner.position;
        Vector3 to = targetWorld;
        Vector3 flat = new Vector3(to.x - from.x, 0f, to.z - from.z);

        if (flat.sqrMagnitude < 0.00001f)
            return;
        
        aimDir = flat.normalized;
        
        // 5. 以玩家pivot为圆心设置位置（公转）
        if (orbitAroundOwner)
        {
            Vector3 pos = owner.position + aimDir * (radius + tipOffset);
            pos.y = owner.position.y + height;
            transform.position = pos;
        }
        
        // 6. 让菱形的朝向指向目标（自转）
        var baseRot = Quaternion.LookRotation(Vector3.up, aimDir);// 保持法线朝上(贴地)，让“平面内的某个本地轴”对齐到 aimDir
        transform.rotation = Quaternion.AngleAxis(yawOffsetInPlane, Vector3.up) * baseRot;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(owner ? owner.position : transform.position, targetWorld);
        Gizmos.DrawSphere(targetWorld, 0.1f);
    }
}
