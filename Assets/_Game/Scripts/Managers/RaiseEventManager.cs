using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaiseEventManager
{
    //Event codes
    public const byte StartTimerEventCode = 1;
    public const byte SyncTimerEventCode = 2;


    public static void RaiseEvent(byte eventCode, params object[] extraParameters)
    {
        if (eventCode == StartTimerEventCode)
        {
            if (extraParameters.Length > 2)
            {
                Debug.LogError("Too many parameters for start timer event.");
                return;
            }
            object[] content = new object[] { extraParameters[0], extraParameters[1] };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache};
            PhotonNetwork.RaiseEvent(StartTimerEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }
        
        if (eventCode == SyncTimerEventCode)
        {
            if (extraParameters.Length > 2)
            {
                Debug.LogError("Too many parameters for sync timer event.");
                return;
            }
            object[] content = new object[] { extraParameters[0], extraParameters[1] };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache};
            PhotonNetwork.RaiseEvent(SyncTimerEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }

}