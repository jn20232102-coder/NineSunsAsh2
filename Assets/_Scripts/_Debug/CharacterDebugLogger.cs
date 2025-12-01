using UnityEngine;

public class CharacterDebugLogger : MonoBehaviour
{
    CharacterBase ch;
    void Awake(){ ch = GetComponent<CharacterBase>(); }
    void OnEnable()
    {
        ch.OnHealthChanged += (cur, max)=> Debug.Log($"HP: {cur}/{max}");
        ch.OnDamaged       += hit        => Debug.Log($"Damaged: {hit.amount} ({hit.type})");
        ch.OnDeath         += ()         => Debug.Log("Death");
        ch.OnInvulnerabilityChanged += v => Debug.Log("Invul=" + v);
    }
    void OnDisable()
    {
        ch.OnHealthChanged -= null; ch.OnDamaged -= null; ch.OnDeath -= null; ch.OnInvulnerabilityChanged -= null;
    }
}
