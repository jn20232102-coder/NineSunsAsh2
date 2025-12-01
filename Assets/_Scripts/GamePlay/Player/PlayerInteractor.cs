using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IInteractable
{
    string Prompt { get; } // 只读属性：提示文本，如“按 E 传送”
    Transform PromptAnchor { get; } // 只读属性：提示UI挂点位置（一般返回 transform）
    void Interact(GameObject actor); // 方法：执行交互（传送/开箱/对话…）
}

[RequireComponent(typeof(Collider))]
public class PlayerInteractor : MonoBehaviour
{
    [Tooltip("交互有效距离（米）")]
    public float maxDistance = 0.5f;

    private readonly List<IInteractable> _inside = new();
    private Transform _self;

    void Awake()
    {
        _self = transform;

        var col = GetComponent<Collider>();
        if (col)
        {
            col.isTrigger = true;
        }
    } 

    void OnEnable()
    {
        // 场景切换时清空，避免残留引用
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        _inside.Clear();
    }

    //调用交互前，剔除已经销毁的目标
    void CullDestroyed()
    {
        for (int i = _inside.Count - 1; i >= 0; i--)
        {
            var comp = _inside[i] as Component; // 接口背后的脚本
            if (comp == null || comp.gameObject == null)
            {
                _inside.RemoveAt(i); // 移除被Destroy的
            }
        }
    }

    public void TryInteract()
    {
        if (_inside.Count == 0) return;

        IInteractable best = null;
        float bestD = float.MaxValue;
        foreach (var it in _inside)
        {
            if (it == null) continue;

            var p = (it as Component).transform.position;
            float d = (p - _self.position).sqrMagnitude;
            if (d < bestD)
            {
                bestD = d;
                best = it;
            }
        }
        if (best != null && bestD <= maxDistance * maxDistance)
        {
            best.Interact(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var it = other.GetComponentInParent<IInteractable>();
        if (it != null && !_inside.Contains(it)) 
            _inside.Add(it);
    }

    void OnTriggerExit(Collider other)
    {
        var it = other.GetComponentInParent<IInteractable>();
        if (it != null) 
            _inside.Remove(it);
    }
}
