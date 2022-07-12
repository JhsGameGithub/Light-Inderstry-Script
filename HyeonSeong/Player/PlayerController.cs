using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    Rigidbody rigid;

    CharacterMover mover;
    CharacterRotator rotator;
    CharacterHandler handler;

    PhotonView pv;
    float horizontal;
    float vertical;

    Vector3 direction;

    private void Awake()
    {
        pv = photonView;

        handler = transform.GetChild(0).GetComponent<CharacterHandler>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        mover = GetComponent<CharacterMover>();
        rotator = GetComponent<CharacterRotator>();
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine)
            return;
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        direction = (Vector3.right * horizontal + Vector3.forward * vertical).normalized;

        if (mover != null)
            mover.CharacterMove(direction, Time.deltaTime, rigid);
        if (rotator != null)
            rotator.CharacterRotate(direction, Time.deltaTime, rigid);
    }
}
