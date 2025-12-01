using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class AutoTextureTiling : MonoBehaviour
{
    public enum AxisMode { XZ, XY, YZ }
    public AxisMode axis = AxisMode.XZ;

    [Tooltip("每一块纹理在世界里覆盖的米数。例如=1 表示 1m 一次重复。")]
    public float metersPerTile = 1f;

    Renderer _r;
    MaterialPropertyBlock _mpb;
    static readonly int _MainTex_ST = Shader.PropertyToID("_MainTex_ST"); // Built-in Standard
    static readonly int _BaseMap_ST = Shader.PropertyToID("_BaseMap_ST"); // URP Lit

    void Ensure()
    {
        if (!_r)   _r = GetComponent<Renderer>();
        if (_mpb == null) _mpb = new MaterialPropertyBlock();
    }

    void OnEnable()   { Ensure(); Apply(); }
    void OnValidate() { Ensure(); Apply(); }
    void LateUpdate() { Apply(); }

    void Apply()
    {
        Ensure();
        if (!_r || metersPerTile <= 0f) return;

        // 用世界空间尺寸（米）
        Vector3 s = _r.bounds.size;
        Vector2 worldSize = axis switch
        {
            AxisMode.XZ => new Vector2(s.x, s.z),
            AxisMode.XY => new Vector2(s.x, s.y),
            _            => new Vector2(s.y, s.z),
        };

        // 计算 Tiling = 世界尺寸 / 每块覆盖米数
        Vector2 tiling = new Vector2(
            Mathf.Max(worldSize.x / metersPerTile, 0.0001f),
            Mathf.Max(worldSize.y / metersPerTile, 0.0001f)
        );

        _r.GetPropertyBlock(_mpb);
        // 仅设置缩放，不改偏移（保持 0,0）
        Vector4 st = new Vector4(tiling.x, tiling.y, 0f, 0f);
        _mpb.SetVector(_MainTex_ST, st);
        _mpb.SetVector(_BaseMap_ST, st);
        _r.SetPropertyBlock(_mpb);
    }
}
