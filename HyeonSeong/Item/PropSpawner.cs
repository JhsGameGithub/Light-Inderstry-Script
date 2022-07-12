using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PropSpawner : MonoBehaviourPun, IPunObservable
{
    private Queue<GameObject> materialQueue = new Queue<GameObject>();

    public GameObject materialPrefab;

    public RecipeManager recipeManager;

    public int currentMaterialNum;

    public int maximumMaterialNum;

    int randomCode;

    IEnumerator spawnDelay;

    PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        spawnDelay = SpawnDelay(2.0f);
        currentMaterialNum = 0;

        maximumMaterialNum = 10;
    }
    //현성 - 재료 소환
    
    public void Spawn()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        //현성 - 재료 코드 생성
       randomCode = Random.Range(0, 3);
        //Generate(randomCode);
        Generate(randomCode);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(name);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();

        }
    }

    //현성 - 재료 생성
    private void Generate(int random)
    {
       
        GameObject material = PhotonNetwork.Instantiate("Product", transform.position, transform.rotation);
       
        Product product = material.GetComponent<Product>();

        product.productCode = random;

        product.SetMaterial(recipeManager);

    }

    [PunRPC]
    private void GenerateRPC(int random)
    {
        GameObject material = PhotonNetwork.Instantiate("Product", transform.position, transform.rotation, 0);
        //GameObject material = Instantiate(materialPrefab, transform.position, transform.rotation);
        Product product = material.GetComponent<Product>();

        product.productCode = random;

        product.SetMaterial(recipeManager);
    }

    //현성 - 스폰 시작
    public void StartSpawn()
    {
        StartCoroutine(spawnDelay);
    }

    //현성 - 스폰 정지
    public void StopSpawn()
    {
        StopCoroutine(spawnDelay);
    }

    IEnumerator SpawnDelay(float timer)
    {
        while (true)
        {
            if (currentMaterialNum < maximumMaterialNum) 
            {
                Spawn();
                currentMaterialNum++;
            }
            yield return new WaitForSeconds(timer);
        }
    }
}
