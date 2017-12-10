using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider2D))]
[RequireComponent (typeof(Rigidbody2D))]
public class Bullet : NetworkBehaviour {
    Rigidbody2D m_rigidbody;

    float m_att;
    float m_timeLeft;

    [SyncVar]
    NetworkInstanceId m_ownerId;

    private void Awake() {
        m_timeLeft = Mathf.Infinity;
    }

    private void Update() {
        m_timeLeft -= Time.deltaTime;
        if(m_timeLeft < 0) {
            Destroy(gameObject);
        }
    }

    public void Init(float att, float speed, float distance, Vector2 direction, NetworkInstanceId ownerId) {
        m_att = att;
        m_rigidbody = gameObject.GetComponent<Rigidbody2D>();
        m_rigidbody.velocity = direction.normalized * speed;
        m_timeLeft = distance / speed;
        m_ownerId = ownerId;
    }

    public override void OnStartClient() {
        base.OnStartClient();

        m_rigidbody = gameObject.GetComponent<Rigidbody2D>();
        Collider2D myCollider = GetComponent<Collider2D>();

        GameObject ownerGO = ClientScene.FindLocalObject(m_ownerId);
        //Debug.Log("OnStartClient : Owner = " + ownerGO == null ? "null" : ownerGO.name);

        if(ownerGO != null) {
            Collider2D ownerCollider = ownerGO.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(myCollider, ownerCollider, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(isServer) {
            HPObject hp = collision.gameObject.GetComponent<HPObject>();
            if(hp != null) {
                hp.AddHP(-m_att);
            }

            NetworkServer.Destroy(gameObject);
        } else {
            gameObject.SetActive(false);
        }
    }
}
