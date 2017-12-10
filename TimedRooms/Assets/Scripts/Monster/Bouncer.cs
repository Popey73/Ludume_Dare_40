using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bouncer : MonoBehaviour {
    [SerializeField]
    Vector2 m_speedRange;

    float m_speed;
    Rigidbody2D m_rigidBody;
    float m_delayTime;

    private void Awake() {
        m_speed = Random.Range(m_speedRange.x, m_speedRange.y);
        m_rigidBody = GetComponent<Rigidbody2D>();
        //m_rigidBody.angularVelocity = 90f;
        m_delayTime = 1.5f;
    }

    private void Update() {
        m_delayTime -= Time.deltaTime;

        if(m_delayTime > 0) {
            m_rigidBody.velocity = Vector2.zero;
            return;
        }

        float velocityMagnitude = m_rigidBody.velocity.magnitude;

        if(velocityMagnitude <= 0.001f) {
            Player player = FindObjectOfType<Player>();
            m_rigidBody.velocity = (player.transform.position - transform.position).normalized * m_speed;
        }

        m_rigidBody.velocity = m_rigidBody.velocity.normalized * m_speed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player" && m_delayTime < 0) {
            collision.gameObject.GetComponent<HPObject>().AddHP(-1);
            m_rigidBody.velocity = -m_rigidBody.velocity;
        }
    }
}
