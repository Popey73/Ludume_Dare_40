using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Door : NetworkBehaviour {
    [SerializeField]
    SpriteRenderer[] m_locks;
    [SerializeField]
    int m_playerCountToOpen;
    [SerializeField]
    Color m_lockedColor;
    [SerializeField]
    Color m_unlockedColor;
    [SerializeField]
    SpriteRenderer m_doorSpriteRenderer;
    [SerializeField]
    Collider2D m_doorCollider;


    int m_currentPlayer;

    private void Awake() {
        for(int i = m_playerCountToOpen ; i < m_locks.Length ; ++i) {
            ActiveLoocker(i, false);
        }
    }

    void AddLock() {
//        Debug.Log("AddLock : " + m_currentPlayer);
        SetLocked(m_currentPlayer-1, true);
    }

    void RemoveLock() {
//        Debug.Log("RemoveLock : " + m_currentPlayer);
        SetLocked(m_currentPlayer, false);
    }

    void SetLocked(int index, bool locked) {
        if(index < m_locks.Length) {
            m_locks[index].color = locked ? m_lockedColor : m_unlockedColor;
        }

        if(locked) {
            m_currentPlayer--;
        } else {
            m_currentPlayer++;
        }
       
        SetDoorOpen(m_currentPlayer >= m_playerCountToOpen);
    }

    void SetDoorOpen(bool open) {
        m_doorSpriteRenderer.enabled = !open;
        m_doorCollider.enabled = !open;
        foreach(SpriteRenderer aLock in m_locks) {
            aLock.gameObject.SetActive(!open);
        }
    }

    void ActiveLoocker(int index, bool active) {
        m_locks[index].enabled = active;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            RemoveLock();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Player") {
            AddLock();
        }
    }
}
