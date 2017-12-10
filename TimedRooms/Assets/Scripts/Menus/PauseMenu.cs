using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class PauseMenu : AbstractMenu {
    public void ResumeButton() {
        DisplayMenu(false);
    }

    public void DisconnectButton() {
        Disconnect();
    }

    public void ExitGameButton() {
        Disconnect();
        Application.Quit();
    }

    public void Disconnect() {
        NetworkManager.singleton.StopHost();
    }
}
