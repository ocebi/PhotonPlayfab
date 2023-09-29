using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    public string Username;
    public PlayerStatisticDictionary PlayerStatisticDictionary = new PlayerStatisticDictionary();
}

[Serializable] public class PlayerStatisticDictionary : UnitySerializedDictionary<PlayerStatistics, int> { }