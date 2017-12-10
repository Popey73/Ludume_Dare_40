using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(HPObject))]
public class Monster : NetworkBehaviour {
    [SerializeField]
    float m_hp;
    [SerializeField]
    GameObject m_rewardOnDeathPrefab;

    private void Awake() {
        //gameObject.GetComponent<HPObject>().CmdInit(m_hp);
    }

    public void OnDeath() {
        if(m_rewardOnDeathPrefab != null) {
            GameObject reward = Instantiate(m_rewardOnDeathPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(reward);
        }
    }
}
