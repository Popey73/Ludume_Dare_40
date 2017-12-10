using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.Networking;

public class EndGameMenu : AbstractMenu {
    [SerializeField]
    Text m_scoreText;

    Player m_player;

    public void DisplayMenu(Player player) {
        m_player = player;
        float time = Time.timeSinceLevelLoad - player.m_spawnTime;

        m_scoreText.text = "Score : " + m_player.m_coin + "\n";
        m_scoreText.text += "Time : " + (int)(time / 60f) + "min " + ((int)time) % 60 + "s" + "\n\n";
        m_scoreText.text += "Best score : " + PlayerPrefs.GetInt("BestScore", 0);

        AnalyticsManager.SendScore(player.m_coin, (int)time);

        DisplayMenu(true);
    }

    public void DisconnectButton() {
        NetworkManager.singleton.StopHost();
    }

    public void RestartButton() {
        m_player.InitPlayer();

        DisplayMenu(false);
    }
}
