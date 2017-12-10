using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class SimpleTurret : NetworkBehaviour {
    
    public float m_bulletSpeed;
    public float m_bulletRange;
    public float m_fireInterval;
    public bool m_fireAllPositions;

    [SyncVar(hook = "OnRotationSpeedChanged")]
    public float m_rotationSpeed;

    [SerializeField]
    bool m_notRandom;
    [SerializeField]
    GameObject m_bulletPrefab;
    [SerializeField]
    Transform[] m_firePositions;

    protected float m_nextBulletTime;

    virtual protected void Awake() {
        if(m_notRandom == false) {
            InitRandom();
        }
    }

    virtual protected void Update() {
        m_nextBulletTime -= Time.deltaTime;

        if(m_nextBulletTime < 0) {
            if(m_fireAllPositions) {
                foreach(Transform firePosition in m_firePositions) {
                    FireFromPosition(firePosition);
                }
            } else {
                RandomFire();
            }
            m_nextBulletTime = m_fireInterval;
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z + Time.deltaTime * m_rotationSpeed));
    }

    virtual public void InitRandom() {
        m_nextBulletTime = 5f;

        m_bulletSpeed = Random.Range(0.5f, 2f);
        m_bulletRange = Random.Range(3f, 10f);
        m_fireAllPositions = Random.Range(0, 3) == 0;
        m_fireInterval = Random.Range(0.08f, 0.5f) * (m_fireAllPositions ? 10f : 1);
        m_rotationSpeed = Random.Range(25f, 120f);
    }

    protected void FireFromPosition(Transform firePosition) {
        Vector3 position = firePosition.position;
        Vector3 direction = position - transform.position;

        Fire(position, direction);
    }

    protected void RandomFire() {
        FireFromPosition(m_firePositions[Random.Range(0, m_firePositions.Length)]);
    }

    protected void Fire(Vector3 position, Vector3 direction) {
        if(isServer) {
            GameObject bulletGO = Instantiate(m_bulletPrefab, position, Quaternion.identity);
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            bullet.Init(1, m_bulletSpeed, m_bulletRange, direction, GetComponent<NetworkIdentity>().netId);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletGO.GetComponent<Collider2D>(), true);
            NetworkServer.Spawn(bulletGO);
        }
    }

    void OnRotationSpeedChanged(float rotationSpeed) {
        m_rotationSpeed = rotationSpeed;
    }
}
