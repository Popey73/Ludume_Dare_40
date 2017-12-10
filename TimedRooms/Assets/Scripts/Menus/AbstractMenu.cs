using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractMenu : MonoBehaviour {
    virtual public void DisplayMenu(bool display) {
        Cursor.visible = display;
        gameObject.SetActive(display);
    }

    public bool IsDisplayed() {
        return gameObject.activeSelf;
    }

    public void CloseMenuButton() {
        DisplayMenu(false);
    }
}
