using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/Create GameConfig", fileName = "GameConfig")]
public class GameConfig : SingletonScriptableObject<GameConfig>
{
    [Header("User Data")] 
    public UserData UserData = new UserData();

    public List<PlayerStatistics> PlayerStatisticsList = new List<PlayerStatistics>();
}