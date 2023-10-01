using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : Singleton<NetworkManager>, IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks, IInRoomCallbacks
{
    public static Action<string> OnErrorReceived;
    private void Start()
    {
        StartJoin();
    }

    protected override void OnEnable() 
    {
        PhotonNetwork.AddCallbackTarget(this);
        MenuManager.Instance.MenuStateMachine.OnStateChanged += OnMenuStateChanged;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        if (!MenuManager.IsInstanceNull)
            MenuManager.Instance.MenuStateMachine.OnStateChanged -= OnMenuStateChanged;
    }

    [Button]
    public void StartJoin()
    {
        PhotonNetwork.NickName = GameConfig.Instance.UserData.Username;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    [Button]
    private void DisconnectPlayer()
    {
        PhotonNetwork.Disconnect();
    }

    private void OnMenuStateChanged(string newState)
    {
        if (newState == nameof(Screen_Menu))
        {
            StartJoin();
        }
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = CreateDefaultRoomOptions();
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    private RoomOptions CreateDefaultRoomOptions()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            CleanupCacheOnLeave = true,
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = GameConfig.Instance.MaxPlayers
        };
        return roomOptions;
    }

    private void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= GameConfig.Instance.MaxPlayers) //Start game
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            
            //Save player statistics to player properties 
            foreach (var keyValuePair in GameConfig.Instance.UserData.PlayerStatisticDictionary)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { keyValuePair.Key.ToString(), (byte)keyValuePair.Value } });    
            }
            
            GameManager.Instance.InitializeGame();
        }
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    #region Connection Callbacks

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.OfflineMode = false;
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        if (MenuManager.IsInstanceNull)
            return;
        GameManager.Instance.FinishGame();
        if (MenuManager.Instance.IsScreenOpened(nameof(Screen_Menu)))
            MenuManager.Instance.OpenScreen(nameof(Screen_Menu));
        if (cause != DisconnectCause.DisconnectByClientLogic) //Display error message
            OnErrorReceived.InvokeSafe(cause.ToString());
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

    #region Matchmaking Callbacks

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnCreatedRoom()
    {
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinedRoom()
    {
        StartGame();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }
    
    #endregion

    #region Lobby Callbacks

    public void OnJoinedLobby()
    {
    }

    public void OnLeftLobby()
    {
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }
    
    #endregion

    #region Room Callbacks
    
    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        StartGame();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (MenuManager.Instance.IsScreenOpened(nameof(Screen_Finish)))
            return;
        GameManager.Instance.FinishGame();
        PhotonNetwork.Disconnect();
        MenuManager.Instance.OpenScreen(nameof(Screen_Menu));
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
    
    #endregion
}