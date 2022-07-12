using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Product : MonoBehaviourPun, IPunObservable
{
    protected MeshRenderer renderer;
    Rigidbody rigid;
    [SerializeField] private GameObject ownObject;
    [SerializeField] private bool isOwning;

    RecipeManager recipe;

    protected int productScore;
    protected int productLevel;
    public int productCode;

    protected virtual void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        isOwning = false;
        ownObject = null;
    }


    private void Update()
    {
        if (isOwning)
        {
            gameObject.transform.position = ownObject.transform.position;
            gameObject.transform.rotation = ownObject.transform.rotation;
        }
    }

    public bool OwnCheck()
    {
        return isOwning;
    }

    public void SetOwn(GameObject own)
    {
        isOwning = true;
        rigid.isKinematic = true;
        ownObject = own;
    }
    
    public void OwnDelete()
    {
        isOwning = false;
        rigid.isKinematic = false;
        ownObject = null;
    }

    public void SetBinder(GameObject binder)
    {
        rigid.isKinematic = true;

        transform.position = binder.transform.position;
        transform.rotation = binder.transform.rotation;
    }

    public void SetMaterial(RecipeManager recipe)
    {
        this.recipe = recipe;
        renderer.material = recipe.GetLevelMaterial(productLevel, productCode);
    }

    public int LevelCheck()
    {
        return productLevel;
    }

    public void SetLevel(int lv)
    {
        productLevel = lv;
    }

    public int GetProductScore()
    {
        return productScore;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(productLevel);
            stream.SendNext(productCode);
        }
        else
        {
            productLevel = (int)stream.ReceiveNext();
            productCode = (int)stream.ReceiveNext();
            SetMaterial(FindObjectOfType<RecipeManager>());
        }
    }
}
