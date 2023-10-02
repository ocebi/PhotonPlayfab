using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using StateSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Screen_Menu : State, IConnectionCallbacks
{
    [SerializeField, ReadOnly]
    private TMP_InputField m_EmailInputField;
    [SerializeField, ReadOnly]
    private TMP_InputField m_PasswordInputField;
    [SerializeField, ReadOnly]
    private TMP_InputField m_NicknameInputField;

    [SerializeField, ReadOnly] 
    private TMP_Text m_InfoText;
    
    [SerializeField, ReadOnly]
    private Button m_ManualLoginButton;
    [SerializeField, ReadOnly]
    private Button m_GuestLoginButton;
    [SerializeField, ReadOnly]
    private Button m_PlayButton;
    [SerializeField, ReadOnly]
    private Button m_LogoutButton;
    [SerializeField, ReadOnly]
    private Button m_SetNicknameButton;
    
    [SerializeField, ReadOnly] 
    private GameObject m_LoggedOutPanel;
    [SerializeField, ReadOnly] 
    private GameObject m_LoggedInPanel;
    
    [Button]
    private void SetRefs()
    {
        m_EmailInputField = transform.FindDeepChild<TMP_InputField>("EmailInputField");
        m_PasswordInputField = transform.FindDeepChild<TMP_InputField>("PasswordInputField");
        m_NicknameInputField = transform.FindDeepChild<TMP_InputField>("NicknameInputField");

        m_InfoText = transform.FindDeepChild<TMP_Text>("InfoText");
        
        m_ManualLoginButton = transform.FindDeepChild<Button>("ManualLoginButton");
        m_GuestLoginButton = transform.FindDeepChild<Button>("GuestLoginButton");
        m_PlayButton = transform.FindDeepChild<Button>("PlayButton");
        m_LogoutButton = transform.FindDeepChild<Button>("LogoutButton");
        m_SetNicknameButton = transform.FindDeepChild<Button>("SetNicknameButton");
        
        m_LoggedInPanel = transform.FindDeepChild<GameObject>("LoggedInPanel");
        m_LoggedOutPanel = transform.FindDeepChild<GameObject>("LoggedOutPanel");
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        PlayfabManager.OnLoginStatusChanged += OnLoginStatusChanged;
        PlayfabManager.OnErrorReceived += OnErrorReceived;
        PlayfabManager.OnMessageReceived += OnMessageReceived;
        NetworkManager.OnErrorReceived += OnErrorReceived;
        m_ManualLoginButton.onClick.AddListener(OnManualLoginButtonClicked);
        m_GuestLoginButton.onClick.AddListener(OnGuestLoginButtonClicked);
        m_PlayButton.onClick.AddListener(OnPlayButtonClicked);
        m_LogoutButton.onClick.AddListener(OnLogoutButtonClicked);
        m_SetNicknameButton.onClick.AddListener(OnSetNicknameButtonClicked);
        SetLoggedIn(PlayfabManager.Instance.IsLoggedIn);
        SetInfoText(GameConfig.Instance.InfoMessageDictionary[InfoMessages.PhotonConnection]);
        m_PlayButton.interactable = false;
        m_GuestLoginButton.interactable = false;
        m_ManualLoginButton.interactable = false;
        if (PlayfabManager.Instance.IsLoggedIn)
            PlayfabManager.Instance.GetStatistics();
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        PlayfabManager.OnLoginStatusChanged -= OnLoginStatusChanged;
        PlayfabManager.OnErrorReceived -= OnErrorReceived;
        PlayfabManager.OnMessageReceived -= OnMessageReceived;
        NetworkManager.OnErrorReceived -= OnErrorReceived;
        m_ManualLoginButton.onClick.RemoveAllListeners();
        m_GuestLoginButton.onClick.RemoveAllListeners();
        m_PlayButton.onClick.RemoveAllListeners();
        m_LogoutButton.onClick.RemoveAllListeners();
        m_SetNicknameButton.onClick.RemoveAllListeners();
    }

    private void SetLoggedIn(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            m_LoggedInPanel.SetActive(true);
            m_LoggedOutPanel.SetActive(false);
            m_NicknameInputField.text = GameConfig.Instance.UserData.Username;
            m_InfoText.SetText("");
        }
        else
        {
            m_LoggedOutPanel.SetActive(true);
            m_LoggedInPanel.SetActive(false);
        }
    }

    private void OnLoginStatusChanged()
    {
        SetLoggedIn(PlayfabManager.Instance.IsLoggedIn);
    }

    private void OnErrorReceived(string errorMessage)
    {
        SetInfoText($"{ExtensionMethods.AddColorToString("Error", Color.red)}: {errorMessage}");
    }

    private void OnMessageReceived(string message)
    {
        SetInfoText(message);
    }

    private void SetInfoText(string text)
    {
        m_InfoText.SetText(text);
    }

    #region Button Clicks
    
    private void OnManualLoginButtonClicked()
    {
        SetInfoText(GameConfig.Instance.InfoMessageDictionary[InfoMessages.Login]);
        var loginData = new LoginData(m_EmailInputField.text, m_PasswordInputField.text);
        PlayfabManager.Instance.Register(loginData);
    }
    
    private void OnGuestLoginButtonClicked()
    {
        SetInfoText(GameConfig.Instance.InfoMessageDictionary[InfoMessages.Login]);
        PlayfabManager.Instance.GuestLogin();
    }

    private void OnPlayButtonClicked()
    {
        PhotonNetwork.NickName = GameConfig.Instance.UserData.Username;
        SetInfoText(GameConfig.Instance.InfoMessageDictionary[InfoMessages.Login]);
        NetworkManager.Instance.JoinRandomRoom();
        MenuManager.Instance.OpenScreen(nameof(Screen_Matchmaking));
    }
    
    private void OnLogoutButtonClicked()
    {
        SetInfoText("");
        PlayfabManager.Instance.Logout();
    }
    
    private void OnSetNicknameButtonClicked()
    {
        PlayfabManager.Instance.UpdateDisplayName(m_NicknameInputField.text);
    }

    #endregion

    #region Connection Callbacks
    
    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        SetInfoText(GameConfig.Instance.InfoMessageDictionary[InfoMessages.PhotonConnected]);
        m_PlayButton.interactable = true;
        m_GuestLoginButton.interactable = true;
        m_ManualLoginButton.interactable = true;
    }

    public void OnDisconnected(DisconnectCause cause)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }
    
    #endregion
}
