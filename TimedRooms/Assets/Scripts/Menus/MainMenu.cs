using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField]
    Text m_bestScoreText;
    [SerializeField]
    Button[] m_buttons;

    private void Start() {
        Cursor.visible = true;
        m_bestScoreText.text = "Best score : " + PlayerPrefs.GetInt("BestScore", 0);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StartMatchMaker();

            foreach(Button button in m_buttons) {
                button.interactable = true;
            }
        }
    }

    public void TutoButton() {
        PlayerPrefs.SetInt("ShowTuto", 1);
        FindObjectOfType<MatchMakerManager>().OffLineButton();
    }

    public void QuitButton() {
        Application.Quit();
    }
}
