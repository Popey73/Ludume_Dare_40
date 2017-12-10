using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TutoPlayZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player") {
            Player player = collision.gameObject.GetComponent<Player>();
            Player.IsTuto = false;
            PlayerPrefs.SetInt("ShowTuto", 0);
            player.InitPlayer();
        }
    }
}
