using Photon.Pun;
using Sirenix.OdinInspector;
using StateSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Screen_Finish : State
{
    [SerializeField, ReadOnly]
    private TMP_Text m_OpponentStatsText;
    [SerializeField, ReadOnly]
    private TMP_Text m_PlayerStatsText;
    [SerializeField, ReadOnly]
    private TMP_Text m_PlayerScoreText;
    [SerializeField, ReadOnly]
    private TMP_Text m_OpponentScoreText;
    [SerializeField, ReadOnly]
    private TMP_Text m_WinnerText;
    
    [SerializeField, ReadOnly]
    private Button m_BackButton;

    [Button]
    private void SetRefs()
    {
        m_OpponentStatsText = transform.FindDeepChild<TMP_Text>("OpponentStatsText");
        m_PlayerStatsText = transform.FindDeepChild<TMP_Text>("PlayerStatsText");
        m_PlayerScoreText = transform.FindDeepChild<TMP_Text>("PlayerScoreText");
        m_OpponentScoreText = transform.FindDeepChild<TMP_Text>("OpponentScoreText");
        m_WinnerText = transform.FindDeepChild<TMP_Text>("WinnerText");
        
        m_BackButton = transform.FindDeepChild<Button>("BackButton");
    }

    private void OnEnable()
    {
        m_BackButton.onClick.AddListener(OnBackButtonClicked);
        SetPlayerStatisticsText();
    }

    private void OnDisable()
    {
        m_BackButton.onClick.RemoveAllListeners();
    }

    private void OnBackButtonClicked()
    {
        PhotonNetwork.Disconnect();
        if (!MenuManager.Instance.IsScreenOpened(nameof(Screen_Menu)))
            MenuManager.Instance.OpenScreen(nameof(Screen_Menu));
    }

    private void SetPlayerStatisticsText()
    {
        m_PlayerStatsText.SetText("");
        m_OpponentStatsText.SetText("");
        m_PlayerScoreText.SetText("");
        m_OpponentScoreText.SetText("");
        foreach (var currentRoomPlayer in PhotonNetwork.CurrentRoom.Players)
        {
            int playerStat = 0;
            string statsText = currentRoomPlayer.Value.NickName + "\n";
            foreach (var playerStatistics in GameConfig.Instance.PlayerStatisticsList)
            {
                if (currentRoomPlayer.Value.CustomProperties.TryGetValue(playerStatistics.ToString(), out var playerStatRaw))
                    playerStat = (byte)playerStatRaw;
                statsText += $"{playerStatistics.ToString()}: {playerStat}\n";
                playerStat = 0;
            }
            if (currentRoomPlayer.Value.IsLocal)
            {
                m_PlayerStatsText.SetText(statsText);
                m_PlayerScoreText.SetText($"{currentRoomPlayer.Value.NickName}\n{GameManager.Instance.GetScore(currentRoomPlayer.Value)}");
            }
            else
            {
                m_OpponentStatsText.SetText(statsText);
                m_OpponentScoreText.SetText($"{currentRoomPlayer.Value.NickName}\n{GameManager.Instance.GetScore(currentRoomPlayer.Value)}");
            }
        }
        if (GameManager.Instance.WinnerPlayer != null)
            m_WinnerText.SetText($"{GameManager.Instance.WinnerPlayer.NickName} Won!");
        else
            m_WinnerText.SetText("Draw");
    }
}