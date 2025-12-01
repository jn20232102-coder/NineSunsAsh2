using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnPoint : MonoBehaviour
{
    [Tooltip("出生点 ID。Portal 的 targetSpawnId 要与之匹配。")]
    public string id = "Default";

    [Tooltip("当没指定 id 或 id 不匹配时，是否作为默认出生点。每个场景建议只勾一个。")]
    public bool isDefault = true;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + (Vector3.up * 0.2f), $"Spawn: {id}{(isDefault ? " (Default)" : "")}");
#endif
    }
}
