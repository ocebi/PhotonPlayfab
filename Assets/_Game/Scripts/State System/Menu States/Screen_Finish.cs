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
        MenuManager.Instance.OpenScreen(nameof(Screen_Menu));
    }

    private void SetPlayerStatisticsText()
    {
        m_PlayerStatsText.SetText("");
        m_OpponentStatsText.SetText("");
        m_PlayerScoreText.SetText("");
        m_OpponentScoreText.SetText("");
        int highScore = 0, winCount = 0, loseCount = 0;
        foreach (var currentRoomPlayer in PhotonNetwork.CurrentRoom.Players)
        {
            if (currentRoomPlayer.Value.CustomProperties.TryGetValue(PlayerStatistics.HighScore.ToString(), out var highScoreObject))
                highScore = (byte)highScoreObject;
            if (currentRoomPlayer.Value.CustomProperties.TryGetValue(PlayerStatistics.WinCount.ToString(), out var winCountObject))
                winCount = (byte)winCountObject;
            if (currentRoomPlayer.Value.CustomProperties.TryGetValue(PlayerStatistics.LoseCount.ToString(), out var loseCountObject))
                loseCount = (byte)loseCountObject;
            string statsText = currentRoomPlayer.Value.NickName + "\n";
            statsText += $"Highscore: {highScore}\nWin count: {winCount}\nLose count: {loseCount}";
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
        m_WinnerText.SetText($"{GameManager.Instance.WinnerPlayer.NickName} Won!");
    }
}