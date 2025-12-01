using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class CameraRig : MonoBehaviour
{
    public static CameraRig Instance { get; private set; }
    
    [Header("Follow")]
    public Vector3 offset = new Vector3(0f, 5.5f , -5f); // 相对玩家位置
    public Vector3 lookEuler = new Vector3(45f, 0f, 0f); // 固定观察角
    // [Range(0.01f, 0.5f)] public float followSmoothTime = 0.15f; // 平滑时间
    [Range(0.01f, 40f)] public float followResponsiveness = 8f; // 位置响应速度（每秒收敛率）
    [Range(0.01f, 20f)] public float rotateResponsiveness = 10f; // 旋转响应速度
    public bool noSmoothingForDebug = false; // 一键关闭平滑做对比

    [Header("Target (Auto)")]
    public Transform target; // 运行时自动查找Player
    

    
    Vector3 _vel;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() => SceneManager.activeSceneChanged += OnSceneChanged;
    void OnDisable() => SceneManager.activeSceneChanged -= OnSceneChanged;

    void Start()
    {
        TryHookPlayer();
    }

    void LateUpdate()
    {
        if (!target) return;

        var wantsPos = target.position + offset;
        var wantsRot = Quaternion.Euler(lookEuler);
        
        // 指数平滑
        // float tp = 1f - Mathf.Exp(-followResponsiveness * Time.deltaTime);
        float tr = 1f - Mathf.Exp(-rotateResponsiveness * Time.deltaTime);
        
        transform.position = wantsPos; // 位置直接跟随（无卡顿感）
        // transform.position = Vector3.Lerp(transform.position, wantsPos, tp);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantsRot, tr);
        
        // // 位置平滑跟随
        // var wants = target.position + offset;
        // transform.position = Vector3.SmoothDamp(transform.position, wants, ref _vel, followSmoothTime);
        //
        // // 固定观察角
        // var wantedRot = Quaternion.Euler(lookEuler);
        // transform.rotation = Quaternion.Slerp(transform.rotation, wantedRot, 0.1f);
    }

    // 场景切换时重新绑定玩家
    void OnSceneChanged(Scene from, Scene to) => TryHookPlayer();
    
    private void TryHookPlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p)
        {
            target = p.transform;
        }
    }

    // 外部强制设置目标
    public void SetTarget(Transform t) => target = t;
}
