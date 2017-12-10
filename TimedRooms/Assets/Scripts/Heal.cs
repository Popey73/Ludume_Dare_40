using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class Heal : NetworkBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if(isServer) {
            if(collision.gameObject.tag == "Player") {
                collision.gameObject.GetComponent<HPObject>().AddHP(3);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
