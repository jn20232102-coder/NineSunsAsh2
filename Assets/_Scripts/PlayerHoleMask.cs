using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoleMask : MonoBehaviour
{
    public Transform player;   // 把 Player 拖进来

    [SerializeField]private Vector3 _playerPos;

    void Update()
    {
        if (player == null) return;
        _playerPos = player.position;
        Shader.SetGlobalVector("_PlayerPos", _playerPos);
    }
}
