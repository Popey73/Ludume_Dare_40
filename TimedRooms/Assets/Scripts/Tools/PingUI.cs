using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

using UnityEngine.UI;

public class PingUI : MonoBehaviour {
    [SerializeField]
    Text m_pingText;

    private void Update() {
        if(NetworkManager.singleton == null || NetworkManager.singleton.isNetworkActive) {
            m_pingText.text = "";
            enabled = false;
        } else {
            m_pingText.text = "Ping : " + NetworkClient.allClients[0].GetRTT();
        }
    }
}
