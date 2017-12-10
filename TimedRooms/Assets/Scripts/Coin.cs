using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider2D))]
public class Coin : NetworkBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        ResolveCollision(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        ResolveCollision(collision.gameObject);
    }

    void ResolveCollision(GameObject other) {
        if(other.tag == "Player") {
            other.GetComponent<Player>().AddCoin(1);
        }
        if(other.tag == "Bullet") {
            NetworkServer.Destroy(other);
        }

        NetworkServer.Destroy(gameObject);
    }
}
