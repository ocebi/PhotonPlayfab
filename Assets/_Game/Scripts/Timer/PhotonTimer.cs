using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonTimer : Timer, IOnEventCallback
{
    [SerializeField, Min(0.01f)]
    private float m_SyncFrequency = 1f;
    private float m_LastSyncTime;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        IsTimerActive = false;
    }

    protected override void Update()
    {
        base.Update();
        if (!IsTimerActive)
            return;
        if (Time.time - m_LastSyncTime >= m_SyncFrequency)
            SyncTimer(ElapsedTime);
    }

    public override void StartTimer(float duration)
    {
        if (PhotonNetwork.IsMasterClient)
            RaiseEventManager.RaiseEvent(RaiseEventManager.StartTimerEventCode, gameObject.name, duration);
    }


    private void SyncTimer(float elapsedTime)
    {
        m_LastSyncTime = Time.time;
        if (!PhotonNetwork.IsMasterClient)
            return;
        RaiseEventManager.RaiseEvent(RaiseEventManager.SyncTimerEventCode, gameObject.name, elapsedTime);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == RaiseEventManager.StartTimerEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string timerName = (string)data[0];
            float timerDuration = (float)data[1];
            if (timerName != gameObject.name)
                return;
            base.StartTimer(timerDuration);
        }
        if (eventCode == RaiseEventManager.SyncTimerEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string timerName = (string)data[0];
            float elapsedTime = (float)data[1];
            if (timerName != gameObject.name)
                return;
            ElapsedTime = elapsedTime;

        }
    }
}