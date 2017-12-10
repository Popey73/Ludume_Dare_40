using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    static private MenuManager m_instance = null;

    public PauseMenu m_pauseMenu;
    public EndGameMenu m_endGameMenu;

    private void Awake() {
        m_pauseMenu.DisplayMenu(false);
        m_endGameMenu.DisplayMenu(false);
    }

    static public MenuManager GetMenuManager() {
        if(m_instance == null) {
            m_instance = FindObjectOfType<MenuManager>();
        }

        return m_instance;
    }
}
