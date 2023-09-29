using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayfabManager : Singleton<PlayfabManager>
{
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
            Debug.LogError("Password is too short");
            return;
        }
        m_LoginData = loginData;

        var request = new RegisterPlayFabUserRequest
        {
            Email = loginData.Email,
            Password = loginData.Password,
            RequireBothUsernameAndEmail = false,
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFail);
    }

    [Button]
    public void Login(LoginData loginData)
    {
        var request = new LoginWithEmailAddressRequest()
        {
            Email = loginData.Email,
            Password = loginData.Password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnPlayFabError);
    }

    [Button]
    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        Debug.LogError("Logged out");
    }
    
    [Button]
    public void UpdatePlayerStatistic(PlayerStatistics playerStatistics, int value)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
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

    #endregion

    #region Responses

    private void OnRegisterSuccess(RegisterPlayFabUserResult registerSuccessResult)
    {
        Debug.LogError("Register success");
        Login(m_LoginData);
    }
    
    private void OnRegisterFail(PlayFabError registerFailResult)
    {
        Debug.LogError($"Register fail. {registerFailResult.Error} - {registerFailResult.ErrorMessage}");
        if (registerFailResult.Error == PlayFabErrorCode.EmailAddressNotAvailable) //Try to login instead
        {
            Login(m_LoginData);
        }
    }

    private void OnLoginSuccess(LoginResult loginResult)
    {
        GameConfig.Instance.UserData = new UserData(); //TODO: To be refactored
        Debug.LogError("Successful login/account creation");
        string name = null;
        if (loginResult.InfoResultPayload.PlayerProfile != null)
            name = loginResult.InfoResultPayload.PlayerProfile.DisplayName;
        if (name == null)
            InitializePlayerData();
        else
            GameConfig.Instance.UserData.Username = name;
        GetStatistics();
    }
    
    private void OnPlayFabError(PlayFabError playfabError)
    {
        Debug.LogError($"Login failed: {playfabError.Error.ToString()}");
    }
    
    private void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult updateUserTitleDisplayNameResult)
    {
        Debug.LogError("Display name updated successfully");
        GameConfig.Instance.UserData.Username = updateUserTitleDisplayNameResult.DisplayName;
    }

    private void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        UserData userData = new UserData();
        if (GameConfig.Instance.UserData != null)
            userData = GameConfig.Instance.UserData;
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            foreach (var playerStatistics in GameConfig.Instance.PlayerStatisticsList)
            {
                if (eachStat.StatisticName == playerStatistics.ToString())
                {
                    userData.PlayerStatisticDictionary.Add(playerStatistics, eachStat.Value);
                    break;
                }
            }
        }
        GameConfig.Instance.UserData = userData;
    }
    
    #endregion

    #region Private Methods

    private void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void InitializePlayerData()
    {
        UpdateDisplayName($"Player_{Random.Range(100000, 1000000)}");
        foreach (var playerStatistics in GameConfig.Instance.PlayerStatisticsList)
        {
            UpdatePlayerStatistic(playerStatistics, 0);
        }
    }

    [Button]
    private void PrintLoggedIn()
    {
        Debug.LogError($"Logged in: {IsLoggedIn}");
    }
    
    #endregion
}

public class LoginData
{
    public string Email;
    public string Password;

    public LoginData(string email, string password)
    {
        Email = email;
        Password = password;
    }
}