using NineSunsAsh.Combat;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStat", menuName = "Configs/CharacterStatsConfig", order = 1)]
public class CharacterStatsConfig : ScriptableObject
{
    [Header("Common")]
    public float moveSpeed = 4f;
    
    [Header("Health")] public float maxHealth;
    public float hurtIFrame; // 受击后无敌帧

    [Header("Resistance(0 = 无减免，1 = 完全免疫)")]  // 类型伤害减免
    [Range(0f, 1f)] public float physicalResist;
    [Range(0f, 1f)] public float fireResist;
    [Range(0f, 1f)] public float poisonResist;

    public float GetResist(DamageType t)
    {
        return t switch
        {
            DamageType.Physical => physicalResist,
            DamageType.Fire => fireResist,
            DamageType.Poison => poisonResist,
            _ => 0f
        };
    }
}
