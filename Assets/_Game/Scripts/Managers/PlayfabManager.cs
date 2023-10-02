using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayfabManager : Singleton<PlayfabManager>
{
    public static Action OnLoginStatusChanged;
    public static Action<string> OnMessageReceived;
    public static Action<string> OnErrorReceived;
    
    public bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();
    private LoginData m_LoginData;
    private UserData m_UserData;

    #region Public Methods
    
    [Button]
    public void GuestLogin()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnPlayFabError);
    }

    [Button]
    public void Register(LoginData loginData)
    {
        if (loginData.Password.Length < 6)
        {
            if (IsSingletonLogsEnabled)
                Debug.LogError("Password is too short");
            OnErrorReceived.InvokeSafe(GameConfig.Instance.InfoMessageDictionary[InfoMessages.PasswordShort]);
            return;
        }
        m_LoginData = loginData;

        var request = new RegisterPlayFabUserRequest
        {
            Username = loginData.UserName,
            Password = loginData.Password,
            RequireBothUsernameAndEmail = false,
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFail);
    }

    [Button]
    public void Login(LoginData loginData)
    {
        var request = new LoginWithPlayFabRequest()
        {
            Username = m_LoginData.UserName,
            Password = m_LoginData.Password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnPlayFabError);
    }

    [Button]
    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        if (IsSingletonLogsEnabled)
            Debug.LogError("Logged out");
        OnLoginStatusChanged.InvokeSafe();
    }

    [Button]
    public void UpdatePlayerStatistic(PlayerStatistics playerStatistics, int value)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            if (IsSingletonLogsEnabled)
                Debug.LogError("Client is not logged in");
            return;
        }
        
        PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest {
                // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
                Statistics = new List<StatisticUpdate> {
                    new StatisticUpdate { StatisticName = playerStatistics.ToString(), Value = value },
                }
            },
            result =>
            {
                if (IsSingletonLogsEnabled)
                    Debug.Log("User statistics updated");
                if (!GameConfig.Instance.UserData.PlayerStatisticDictionary.ContainsKey(playerStatistics))
                {
                    GameConfig.Instance.UserData.PlayerStatisticDictionary.Add(playerStatistics, value);
                }
            },
            error =>
            {
                if (IsSingletonLogsEnabled)
                    Debug.LogError(error.GenerateErrorReport());
            });
    }

    [Button]
    public void UpdateDisplayName(string playerName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = playerName
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdateSuccess, OnPlayFabError);
    }
    
    public void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => { if (IsSingletonLogsEnabled) Debug.LogError(error.GenerateErrorReport()); } 
        );
    }

    #endregion

    #region Responses

    private void OnRegisterSuccess(RegisterPlayFabUserResult registerSuccessResult)
    {
        Login(m_LoginData);
    }
    
    private void OnRegisterFail(PlayFabError registerFailResult)
    {
        if (registerFailResult.Error == PlayFabErrorCode.UsernameNotAvailable) //Try to login instead
            Login(m_LoginData);
        else
            OnErrorReceived.InvokeSafe(registerFailResult.Error.ToString());
    }

    private void OnLoginSuccess(LoginResult loginResult)
    {
        GameConfig.Instance.UserData = new UserData();
        string name = null;
        if (loginResult.InfoResultPayload.PlayerProfile != null)
            name = loginResult.InfoResultPayload.PlayerProfile.DisplayName;
        if (name == null)
            InitializePlayerData();
        else
            GameConfig.Instance.UserData.Username = name;
        GetStatistics();
        OnLoginStatusChanged.InvokeSafe();
    }
    
    private void OnPlayFabError(PlayFabError playfabError)
    {
        if (IsSingletonLogsEnabled)
            Debug.LogError($"Playfab error: {playfabError.Error.ToString()}");
        OnErrorReceived.InvokeSafe(playfabError.Error.ToString());
    }
    
    private void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult updateUserTitleDisplayNameResult)
    {
        GameConfig.Instance.UserData.Username = updateUserTitleDisplayNameResult.DisplayName;
        OnMessageReceived.InvokeSafe(GameConfig.Instance.InfoMessageDictionary[InfoMessages.NameSet]);
    }

    private void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        if (IsSingletonLogsEnabled)
            Debug.Log("Received the following Statistics:");
        UserData userData = new UserData();
        if (GameConfig.Instance.UserData != null)
            userData = GameConfig.Instance.UserData;
        foreach (var eachStat in result.Statistics)
        {
            if (IsSingletonLogsEnabled)
                Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            foreach (var playerStatistics in GameConfig.Instance.PlayerStatisticsList)
            {
                if (eachStat.StatisticName != playerStatistics.ToString()) continue;
                
                if (!userData.PlayerStatisticDictionary.ContainsKey(playerStatistics))
                    userData.PlayerStatisticDictionary.Add(playerStatistics, eachStat.Value);
                else
                    userData.PlayerStatisticDictionary[playerStatistics] = eachStat.Value;
                break;
            }
        }
        GameConfig.Instance.UserData = userData;
    }

    #endregion

    #region Private Methods

    private void InitializePlayerData()
    {
        string name = $"Player_{Random.Range(100000, 1000000)}";
        GameConfig.Instance.UserData.Username = name;
        UpdateDisplayName(name);
        foreach (var playerStatistics in GameConfig.Instance.PlayerStatisticsList)
        {
            UpdatePlayerStatistic(playerStatistics, 0);
        }
    }
    
    #endregion
}

public class LoginData
{
    public string UserName;
    public string Password;

    public LoginData(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }
}