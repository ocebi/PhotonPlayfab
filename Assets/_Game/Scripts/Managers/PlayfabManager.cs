using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabManager : MonoBehaviour
{
    [ContextMenu("Guest Login")]
    private void GuestLogin()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFail);
    }

    private void OnLoginSuccess(LoginResult loginResult)
    {
        Debug.LogError("Succesfull login/account creation");
    }
    
    private void OnLoginFail(PlayFabError playfabError)
    {
        Debug.LogError($"Login failed: {playfabError.Error.ToString()}");
    }
}