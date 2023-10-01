using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using StateSystem;
using UnityEngine;
using UnityEngine.UI;

public class Screen_Matchmaking : State
{
    [SerializeField, ReadOnly]
    private Button m_MatchmakingCancelButton;

    [Button]
    private void SetRefs()
    {
        m_MatchmakingCancelButton = transform.FindDeepChild<Button>("MatchmakingCancelButton");
    }

    private void OnEnable()
    {
        m_MatchmakingCancelButton.onClick.AddListener(OnMatchmakingCancelButtonClicked);
    }

    private void OnDisable()
    {
        m_MatchmakingCancelButton.onClick.RemoveAllListeners();
    }
    
    private void OnMatchmakingCancelButtonClicked()
    {
        // if (PhotonNetwork.IsConnected)
        //     PhotonNetwork.Disconnect();
        // else
        // {
            PhotonNetwork.Disconnect();
            MenuManager.Instance.OpenScreen(nameof(Screen_Menu));
        // }
    }
}