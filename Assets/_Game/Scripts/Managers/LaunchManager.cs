using System;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public const string PhotonPrefabFolder = "Photon Prefabs";

    private void Start()
    {
        StartJoin();
    }

    [Button]
    public void StartJoin()
    {
        PhotonNetwork.NickName = "User" + UnityEngine.Random.Range(1, 1000);
        PhotonNetwork.ConnectUsingSettings();
        
    }
    
    [Button]
    private void DisconnectPlayer()
    {
        PhotonNetwork.Disconnect();
    }
    
    void CreatePlayerManager()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        PhotonNetwork.Instantiate(Path.Combine(LaunchManager.PhotonPrefabFolder, "PlayerSpawnManager"), Vector3.zero, Quaternion.identity);
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.OfflineMode = false;
        Debug.Log("Total Players: " + PhotonNetwork.CountOfPlayers);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenScreen(nameof(Screen_Menu));
        // JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        StartGame();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }
    #endregion

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
            MaxPlayers = 15
        };
        return roomOptions;
    }

    private void StartGame()
    {
        // PhotonNetwork.IsMessageQueueRunning = false;
        // SceneManager.LoadScene(GameSceneIndex);
        Debug.LogError($"Joined room. Player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        CreatePlayerManager();
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
}