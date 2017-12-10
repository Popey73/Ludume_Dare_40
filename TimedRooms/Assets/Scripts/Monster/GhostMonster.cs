using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class GhostMonster : NetworkBehaviour {
    [SerializeField]
    float m_speed;

    List<GameObject> m_playersDetected;
    float m_spawnNoEffectTime;

    GameObject m_target;
    float m_delay;

    private void Awake() {
        m_playersDetected = new List<GameObject>();
        m_delay = 2f;
        m_spawnNoEffectTime = 1f;
    }

    private void Update() {
        if(isServer) {
            m_spawnNoEffectTime -= Time.deltaTime;

            if(m_target != null && m_delay <= 0) {
                Vector3 direction = m_target.transform.position - transform.position;
                direction = direction.normalized;

                transform.Translate(direction * Time.deltaTime * m_speed);
            } else {
                m_delay -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(isServer) {
            if(collision.gameObject.tag == "Player") {
                if(m_playersDetected.Contains(collision.gameObject) == false) {
                    m_playersDetected.Add(collision.gameObject);
                    UpdateTarget();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(isServer) {
            if(collision.gameObject.tag == "Player") {
                m_playersDetected.Remove(collision.gameObject);
                UpdateTarget();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(isServer) {
            if(collision.gameObject.tag == "Bullet") {
                m_delay = 0.15f;
            } else if(collision.gameObject.tag == "Player" && m_spawnNoEffectTime < 0) {
                collision.gameObject.GetComponent<HPObject>().AddHP(-1);
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    void UpdateTarget() {
        if(isServer) {
            if(m_playersDetected.Count > 0) {
                m_target = m_playersDetected[0];
            } else {
                m_target = null;
            }
        }
    }
}
