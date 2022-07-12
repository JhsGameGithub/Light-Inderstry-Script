using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterRotator : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    public float RotateSpeed = 10.0f;

    private void Awake()
    {
        pv = photonView;
    }
    //캐릭터 회전
    public void CharacterRotate(Vector3 direction, float deltaTime, Rigidbody rigid)
    {
        if (pv.IsMine)
        {
            if (direction != Vector3.zero)
            {
                Quaternion look = Quaternion.LookRotation(direction);

                rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, look, RotateSpeed * deltaTime));
            }
        }
    }
}
