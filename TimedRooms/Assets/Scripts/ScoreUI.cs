using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour{
    [SerializeField]
    Text m_scoreText;

    Player m_player;

    public void SetPlayer(Player player) {
        m_player = player;
//        (transform as RectTransform).position = Vector3.zero;
    }

    void Update() {
        if(m_player != null) {
            UpdateUI();
        }
    }

    public void UpdateUI() {
        string scoreString = "";

        scoreString += "<color=#BB1111>Score : " + m_player.m_coin + "</color>\n";
        //scoreString += "Att : " + m_player.m_att + "\n";
        scoreString += "<color=green>PV : " + m_player.m_hpObject.m_hp + "/" + m_player.m_hpObject.m_maxHp + "</color>\n";
        scoreString += "<color=blue>Shield : " + (int)m_player.m_shield + "." + (int)((m_player.m_shield * 10) % 10) + "s/" + m_player.m_maxShield + "s</color>";
        //scoreString += "Speed : " + m_player.m_speed + "\n";
        //scoreString += "Fire rate : " + m_player.m_fireRate + "\n";
        //scoreString += "Bullet speed : " + m_player.m_fireSpeed + "\n";
        //scoreString += "Bullet range : " + m_player.m_fireRange + "\n";

        m_scoreText.text = scoreString;
    }
}
