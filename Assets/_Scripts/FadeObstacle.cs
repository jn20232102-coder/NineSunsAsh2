using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FadeObstacle : MonoBehaviour
{
    public  FadeObstacleConfig config;

    Renderer _renderer;
    MaterialPropertyBlock _mpb;

    float _currentFade = 1f;
    float _targetFade = 1f;
    bool _usingTransparent = false;

    void Awake()
    {
        // 建议挂在 TreeRoot 上，这里找子物体的 SpriteRenderer/Renderer
        _renderer = GetComponentInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();

        // // 如果没手动填 opaqueMaterial，就用当前材质当 Opaque
        // if (config.opaqueMaterial == null && _renderer != null)
        //     config.opaqueMaterial = _renderer.sharedMaterial;

        // // 初始化为不透明材质
        // if (_renderer != null && config.opaqueMaterial != null)
        //     _renderer.sharedMaterial = config.opaqueMaterial;
        if (_renderer != null && config != null)
        {
            if (config.opaqueMaterial != null)
                _renderer.sharedMaterial = config.opaqueMaterial;
        }
    }

    void Update()
    {
        if (!_usingTransparent || config == null) return;   // 只有透明材质时才需要更新 _Fade

        if (Mathf.Approximately(_currentFade, _targetFade))
        {
            // 已经完全回到 1，并且目标也是 1 → 可以切回 Opaque
            if (_targetFade >= 0.999f && _currentFade >= 0.999f)
                SwitchToOpaque();
            return;
        }

        _currentFade = Mathf.MoveTowards(
            _currentFade, _targetFade, config.fadeSpeed * Time.deltaTime);

        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Fade", _currentFade);
        _renderer.SetPropertyBlock(_mpb);

        // 回到完全不透明时，切回 Opaque 材质
        if (_targetFade >= 0.999f && _currentFade >= 0.999f)
            SwitchToOpaque();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SwitchToTransparent();
        _targetFade = config.fadedValue;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 先把 Fade 慢慢推回 1，推到 1 之后在 Update 里自动切回 Opaque
        _targetFade = 1f;
    }

    void SwitchToTransparent()
    {
        if (_renderer == null || config.transparentMaterial == null) return;
        if (_usingTransparent) return;

        _renderer.sharedMaterial = config.transparentMaterial;
        _usingTransparent = true;

        // 进入透明模式时先保证当前 Fade 是 1（从“正常树”开始渐隐）
        _currentFade = 1f;
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Fade", _currentFade);
        _renderer.SetPropertyBlock(_mpb);
    }

    void SwitchToOpaque()
    {
        if (_renderer == null || config.opaqueMaterial == null) return;
        if (!_usingTransparent) return;

        _usingTransparent = false;
        _renderer.sharedMaterial = config.opaqueMaterial;
    }
}
