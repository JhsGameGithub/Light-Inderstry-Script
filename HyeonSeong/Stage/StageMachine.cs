using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//-------------------------------- 신규코드 안될시 삭제
//using Photon.Pun;
//using Photon.Realtime;
//--------------------------------
public class StageMachine : MonoBehaviourPun//Pun, IPunObservable // pun부터 삭제
{
    //현성 - 스테이지 타이머
    public float stageTimer;

    float currentRemainingTime;
    float totalTime;

    //현성 - 현재 스테이지 넘버
    int currentStage;

    //현성 - 제출한 제품
    int submissionProduct;

    //현성 - 제출 해야 할 제품의 수 (랜덤 생성)
    int currentProductNum;

    //현성 - 전체 제품의 제출 해야 할 제품의 수
    int maximumProeuctNum;

    //현성 - 제출 해야 할 제품의 수를 저장
    public int[] requestProduct = new int[3];

    //현성 - 스포너
    public PropSpawner propSpawner;

    //현성 - 게임오버 타이머
    IEnumerator gameOverTimer;

    //현성 - IngameUI
    public IngameUI scoreUI;

    //현성 - StartButton
    public GameObject startBtn;

    PhotonView pv;

    bool isStart = false;

    //테스트용 안될시 삭제
    //----------------------------------------
    /*
    private PhotonView pv;
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
    */
    //----------------------------------------

    //제출할 제품의 갯수 종류별로 랜덤
    void RequestProduct()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        maximumProeuctNum = currentStage;

        requestProduct[0] = 0;
        requestProduct[1] = 0;
        requestProduct[2] = 0;

        int num;
        for(int i = 0; i < currentStage; i++)
        {
            num = Random.Range(0, 3);
            requestProduct[num]++;
        }

        pv.RPC("UpdateRequest", RpcTarget.Others, requestProduct[0], requestProduct[1], requestProduct[2]);
    }

    [PunRPC]
    void UpdateRequest(int first, int second, int third)
    {
        requestProduct[0] = first;
        requestProduct[1] = second;
        requestProduct[2] = third;


        scoreUI.SetQuestText(requestProduct[0], requestProduct[1], requestProduct[2]);
    }

    private void Awake()
    {
        currentStage = 1;
        maximumProeuctNum = 1;
        submissionProduct = 0;
        stageTimer = 180.0f;
        totalTime = 0.0f;

        pv = GetComponent<PhotonView>();
        //안될시 삭제------------------
        //pv = GetComponent<PhotonView>();
        //-------------------------
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            startBtn.SetActive(true);
        //StageStart();
    }

    private void Update()
    {
        if (!isStart)
            return;
        currentRemainingTime -= Time.deltaTime;
        totalTime += Time.deltaTime;
        scoreUI.SetTimerText(currentRemainingTime);
    }

    //현성 - 제품 제출
    public void SubmissionProduct(Product product)
    {
        if (product.LevelCheck() > 0)
        {
            Debug.Log(product.productCode);
            Debug.Log(requestProduct[product.productCode]);
            if (requestProduct[product.productCode] != 0)
            {
                requestProduct[product.productCode]--;
                submissionProduct++;
                scoreUI.SetQuestText(requestProduct[0], requestProduct[1], requestProduct[2]);
            }
        }

        if (submissionProduct == currentStage)
            StageEnd();
    }

    //현성 - 스테이지 시작
    [PunRPC]
    public void StageStart()
    {
        //현성 - 만들어야 할 제품 갱신
        RequestProduct();

        //현성 - 현재 남은시간 초기화
        currentRemainingTime = stageTimer;

        //현성 - UI초기화
        scoreUI.GameStart(requestProduct[0], requestProduct[1], requestProduct[2], currentRemainingTime);

        //현성 - 타이머 초기화
        gameOverTimer = GameOverTimer(stageTimer);

        //현성 - 스테이지의 제한시간을 가동
        StartCoroutine(gameOverTimer);

        //현성 - 프롭스포너 스폰 시작
        propSpawner.StartSpawn();

        isStart = true;

        //현성 - 게임시작 버튼 비활성화
        if (PhotonNetwork.IsMasterClient)
            startBtn.SetActive(false);
    }

    public void StageStartRPC()
    {
        pv.RPC("StageStart", RpcTarget.Others);
    }

    //현성 - 스테이지 끝
    void StageEnd()
    {
        //현성 - 패배조건 검사(변형자유)
        if (currentStage == 5)
        {
            pv.RPC("GameClear", RpcTarget.All);
        }

        //현성 - 총 제출한 제품 수 초기화
        submissionProduct = 0;

        //현성 - 게임오버 제한시간 정지
        StopCoroutine(gameOverTimer);

        //현성 - 프롭스포너 스폰 정지
        propSpawner.StopSpawn();

        //현성 - 게임시작 버튼 활성화
        if (PhotonNetwork.IsMasterClient)
            startBtn.SetActive(true);

        //현성 - 스테이지를 올림
        currentStage++;

        //현성 - 다음 스테이지를 시작
        //StageStart();

        isStart = false;
    }
    //현성 - 게임 오버
    [PunRPC]
    void GameOver()
    {
        Debug.Log("GameOver");

        isStart = false;
        scoreUI.GameOver();
    }

    //현성 - 게임 클리어
    [PunRPC]
    void GameClear()
    {
        Debug.Log("GameClear");

        isStart = false;
        scoreUI.GameClear();

        scoreUI.SetClearTimeText(totalTime);
    }

    IEnumerator GameOverTimer(float timer)
    {
        yield return new WaitForSeconds(timer);
        pv.RPC("GameOver", RpcTarget.All);
        //GameOver();
    }
}
