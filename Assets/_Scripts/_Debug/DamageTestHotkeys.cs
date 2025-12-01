using NineSunsAsh.Combat;
using UnityEngine;

public class DamageTestHotkeys : MonoBehaviour
{
    // —— 仅用于在 Inspector 显示使用说明 —— 
    [SerializeField, TextArea(1, 4)] private string help =
        "H: 10 物理伤害（受无敌影响）\n" +
        "J: 10 火焰伤害（忽略无敌）\n" +
        "K: 强击退\n" + 
        "U: 开启 2 秒无敌（Scripted）\n" +
        "I: 结束 Scripted 无敌";
    
    public CharacterBase target;  // 指向玩家

    public float damage = 10f;
    public float knockbackForce = 5f;
    public float invulnerableDuration = 2f;
    
    
    void Update()
    {
        if (!target) return;

        if (Input.GetKeyDown(KeyCode.H))  // 普通打击（受无敌影响）
            target.ReceiveHit(new HitStructure(damage, transform.position, knockbackForce, gameObject));

        if (Input.GetKeyDown(KeyCode.J))  // 无视无敌
            target.ReceiveHit(new HitStructure(damage, transform.position, 0f, gameObject)
                {type = DamageType.Fire, ignoreInvulnerability = true});
        
        if (Input.GetKeyDown(KeyCode.K))  // 强击退
            target.ReceiveHit(new HitStructure(damage, transform.position, 20f, gameObject)
                {blockBreakingPower = 5}); // 破防力5层

        if (Input.GetKeyDown(KeyCode.U))  // 手动开2秒无敌
            target.BeginIFrame(invulnerableDuration, InvulnerabilityFrameSource.Scripted);

        if (Input.GetKeyDown(KeyCode.I))  // 结束无敌
            target.EndIFrame(InvulnerabilityFrameSource.Scripted);
    }
    
#if UNITY_EDITOR
    void OnValidate() { _ = help; } // 消除 CS0414 报错
#endif
}


