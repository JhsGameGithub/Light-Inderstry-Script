using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private GameObject QuestUI;
    [SerializeField] private TextMeshProUGUI QuestYellowText;
    [SerializeField] private TextMeshProUGUI QuestCyanText;
    [SerializeField] private TextMeshProUGUI QuestMagentaText;
    [SerializeField] private TextMeshProUGUI QuestTimerText;

    [SerializeField] private GameObject ClearUI;
    [SerializeField] private Text ClearTime;

    [SerializeField] private GameObject OverUI;

    public void GameStart(int yellow, int cyan, int magenta, float timer)
    {
        SetQuestText(yellow, cyan, magenta);
        SetTimerText(timer);
        QuestUI.SetActive(true);
    }

    public void SetQuestText(int matYellow, int matCyan, int matMagenta)
    {
        QuestYellowText.text = matYellow.ToString("00");
        QuestCyanText.text = matCyan.ToString("00");
        QuestMagentaText.text = matMagenta.ToString("00");
    }

    public void SetTimerText(float time)
    {
        QuestTimerText.text = ((int)(time / 60.0f)).ToString("00") + " : " + ((int)(time % 60.0f)).ToString("00");
    }

    public void SetClearTimeText(float time)
    {
       ClearTime.text = ((int)(time / 60.0f)).ToString("00") + " : " + ((int)(time % 60.0f)).ToString("00");
    }

    public void GameClear()
    {
        ClearUI.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public void GameOver()
    {
        OverUI.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LoadLevel("RoomScene");
    }
}
