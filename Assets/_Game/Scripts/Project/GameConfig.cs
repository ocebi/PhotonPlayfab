using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/Create GameConfig", fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    [Header("Session")] 
    [ReadOnly]
    public UserData UserData = new UserData();

    [Header("Match Data")] 
    [Min(1)]
    public int MaxPlayers = 2;
    public int InitialCountdown = 5;
    public int MatchDuration = 30;

    [Header("Playfab")]
    public List<PlayerStatistics> PlayerStatisticsList = new List<PlayerStatistics>();
    
    [Header("Menu")]
    public InfoMessageDictionary InfoMessageDictionary = new InfoMessageDictionary();
}

[Serializable] public class InfoMessageDictionary : UnitySerializedDictionary<InfoMessages, string> { }