using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : Singleton<GameManager>, IInRoomCallbacks
{
    public static Action OnGameStarted;
    public static Action OnGameFinished;
    public Dictionary<Player, int> PlayerScoreDict = new Dictionary<Player, int>();
    public Player WinnerPlayer { get; private set; }

    public bool IsGameStarted { get; private set; }

    protected override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void InitializeGame()
    {
        PlayerScoreDict.Clear();
        MenuManager.Instance.OpenScreen(nameof(Screen_Gameplay));
    }

    public void StartGame()
    {
        OnGameStarted.InvokeSafe();
        IsGameStarted = true;
    }

    public void FinishGame()
    {
        if (!IsGameStarted)
            return;
        SendScores();
        OnGameFinished.InvokeSafe();
        IsGameStarted = false;
    }

    private void SendScores()
    {
        var currentScore = ButtonController.Instance.ClickCount;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "CurrentScore", (byte)currentScore } });
        if (!GameConfig.Instance.UserData.PlayerStatisticDictionary.ContainsKey(PlayerStatistics.HighScore) ||
            GameConfig.Instance.UserData.PlayerStatisticDictionary[PlayerStatistics.HighScore] < currentScore)
        {
            PlayfabManager.Instance.UpdatePlayerStatistic(PlayerStatistics.HighScore, currentScore);
        }
    }

    public int GetScore(Player player)
    {
        if (PlayerScoreDict.ContainsKey(player))
            return PlayerScoreDict[player];
        return 0;
    }

    #region In Room Callbacks

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!changedProps.TryGetValue("CurrentScore", out var currentScore))
            return;
        int score = (byte)currentScore;
        if (!PlayerScoreDict.ContainsKey(targetPlayer))
            PlayerScoreDict.Add(targetPlayer, score);
        if (PlayerScoreDict.Count >= GameConfig.Instance.MaxPlayers)
        {
            Player winnerPlayer = null;
            int highestScore = 0;
            foreach (var keyValuePair in PlayerScoreDict)
            {
                if (keyValuePair.Value > highestScore)
                {
                    winnerPlayer = keyValuePair.Key;
                    highestScore = keyValuePair.Value;
                }
            }

            if (winnerPlayer.IsLocal)
            {
                var currentWinCount = 0;
                if (GameConfig.Instance.UserData.PlayerStatisticDictionary.ContainsKey(PlayerStatistics.WinCount))
                    currentWinCount = GameConfig.Instance.UserData.PlayerStatisticDictionary[PlayerStatistics.WinCount];
                PlayfabManager.Instance.UpdatePlayerStatistic(PlayerStatistics.WinCount, ++currentWinCount);
            }
            else
            {
                var currentLoseCount = 0;
                if (GameConfig.Instance.UserData.PlayerStatisticDictionary.ContainsKey(PlayerStatistics.LoseCount))
                    currentLoseCount = GameConfig.Instance.UserData.PlayerStatisticDictionary[PlayerStatistics.LoseCount];
                PlayfabManager.Instance.UpdatePlayerStatistic(PlayerStatistics.LoseCount, ++currentLoseCount);
            }

            WinnerPlayer = winnerPlayer;
            MenuManager.Instance.OpenScreen(nameof(Screen_Finish));
        }
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
    
    #endregion
}
