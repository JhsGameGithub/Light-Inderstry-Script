using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterMover : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    public float MoveSpeed = 10.0f;

    private void Awake()
    {
        pv = photonView;
    }

    //캐릭터 이동
    public void CharacterMove(Vector3 direction, float deltaTime, Rigidbody rigid)
    {
        if(pv.IsMine)
            rigid.MovePosition(transform.position + direction * deltaTime * MoveSpeed);
    }
}
