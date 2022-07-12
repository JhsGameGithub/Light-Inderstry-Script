using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterHandler : MonoBehaviourPun, IPunObservable
{
    PhotonView pv;
    public Product thing;
    public GameObject preThing;

    public bool isHandling;


    private void Awake()
    {
        pv = photonView;
    }

    private void Start()
    {
        thing = null;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Handling();
                pv.RPC("HandlingRPC", RpcTarget.Others);
                //pv.RPC("Handling", RpcTarget.AllViaServer);
            }
        }
    }
    public void Handling()
    {   
        if (!isHandling)
        {
            if (preThing != null)
            {
                Debug.Log(preThing.GetComponent<Product>());
                //소유권이 있는지 체크
                if (!preThing.GetComponent<Product>().OwnCheck())
                {
                    Debug.Log("Own Check!");
                    thing = preThing.GetComponent<Product>();
                    thing.SetOwn(gameObject);
                    isHandling = true;
                   
                }
            }
        }
        else
        {
            Debug.Log("Thing!");
            thing.OwnDelete();
            thing = null;
            isHandling = false;
            preThing = null;
           
        }
    }

    [PunRPC]
    private void HandlingRPC()
    {
        if (!isHandling)
        {
            if (preThing != null)
            {
                Debug.Log("Product != NULL");
                //소유권이 있는지 체크
                if (!preThing.GetComponent<Product>().OwnCheck())
                {
                    Debug.Log("Own Check!");
                    thing = preThing.GetComponent<Product>();
                    thing.SetOwn(gameObject);
                    isHandling = true;
                }
            }
        }
        else
        {
            Debug.Log("Thing!");
            thing.OwnDelete();
            thing = null;
            isHandling = false;
            preThing = null;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && preThing != null)
        {
            stream.SendNext(preThing.transform.position);
            stream.SendNext(preThing.transform.rotation);
            stream.SendNext(preThing.name);
           
        }
        else
        {
            preThing.transform.position = (Vector3)stream.ReceiveNext();
            preThing.transform.rotation = (Quaternion)stream.ReceiveNext();

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (preThing == null)
            preThing = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (preThing == other.gameObject)
            preThing = null;
    }
}
