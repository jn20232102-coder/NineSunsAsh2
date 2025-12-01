using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/FadeObstacleConfig")]
public class FadeObstacleConfig : ScriptableObject
{
    [Header("Fade 设置")]
    public float fadedValue = 0.3f;
    public float fadeSpeed = 5f;

    [Header("材质")]
    public Material opaqueMaterial;       // 平时用的（Opaque，有阴影）
    public Material transparentMaterial;  // 虚化用的（Transparent，有 _Fade）

}
