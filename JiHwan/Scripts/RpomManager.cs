using UnityEngine;
using Photon.Pun;

public class RpomManager : MonoBehaviour
{
    void Start()
    {
        PhotonInit.Instance.isGameStart = false;
        PhotonInit.Instance.LobbyCanvas.SetActive(true);
        PhotonNetwork.JoinLobby();
        PhotonInit.Instance.MyListRenewal();
    }
}
