using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

using UnityEngine.UI;

public class MatchMakerManager : MonoBehaviour {

    //United States : us1-mm.unet.unity3d.com  107.21.45.54
    //Europe : eu1-mm.unet.unity3d.com 18.194.147.10
    //Singapore : ap1-mm.unet.unity3d.com 52.76.172.239

    [SerializeField]
    Button[] m_buttons;

    const string QuickRoomName = "TimedRoomsQuick";

    string m_currentRoomeName;

    NetworkClient m_networkClient;

    void Start() {
        NetworkManager.singleton.StartMatchMaker();

        //if(Application.isEditor) {
        //    Tools.PrintPing("107.21.45.54", "United States");
        //    Tools.PrintPing("18.194.147.10", "Europe");
        //    Tools.PrintPing("52.76.172.239", "Singapore");
        //}
    }

    public void QuickPlayButton() {
        EnableButtons(false);
        FindInternetMatch(QuickRoomName);
    }

    public void CreateServerButton() {
        CreateInternetMatch(QuickRoomName);
    }

    public void OffLineButton() {
        EnableButtons(false);
        NetworkManager.singleton.StartHost();
    }

    void OnConnected(NetworkMessage netMsg) {
        Debug.Log("Connected : " + netMsg.ToString());
    }

    public void CreateInternetMatch(string matchName) {
        uint playerCount = matchName == QuickRoomName ? 25u : 100u;
        NetworkManager.singleton.matchMaker.CreateMatch(matchName, playerCount, true, "", "", "", 0, 0, OnInternetMatchCreate);
    }

    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
        if(success) {
            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(hostInfo, 9000);

            NetworkManager.singleton.StartHost(hostInfo);
        } else {
            EnableButtons(true);
            Debug.LogError("Create match failed : " + extendedInfo);
        }
    }

    public void FindInternetMatch(string matchName) {
        m_currentRoomeName = matchName;
        NetworkManager.singleton.matchMaker.ListMatches(0, 100, matchName, true, 0, 0, OnInternetMatchList);
    }

    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
        if(success) {
            if(matches.Count != 0) {
                Debug.Log("Maths count = " + matches.Count);

                for(int i = matches.Count - 1 ; i >= 0 ; i++) {
                    MatchInfoSnapshot info = matches[i];
                    if(info.currentSize > 0 && info.currentSize < info.maxSize) {
                        NetworkManager.singleton.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
                        return;
                    }
                }

                CreateInternetMatch(m_currentRoomeName);
            } else {
                CreateInternetMatch(m_currentRoomeName);
            }
        } else {
            EnableButtons(true);
            Debug.LogError("Couldn't connect to match maker : " + extendedInfo);
        }
    }

    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo) {
        if(success) {
            MatchInfo hostInfo = matchInfo;
            NetworkManager.singleton.StartClient(hostInfo);
        } else {
            EnableButtons(true);
            Debug.LogError("Join match failed : " + extendedInfo);
        }
    }

    void EnableButtons(bool enable) {
        foreach(Button button in m_buttons) {
            button.interactable = enable;
        }
    }

    public static bool IsNetworkActive() {
        return NetworkManager.singleton != null && NetworkManager.singleton.isNetworkActive; 
    }
}
