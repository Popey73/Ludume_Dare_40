using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CoinSpawner : NetworkBehaviour {
    const int InitialSpawnCount = 20;
    const float SpawnDelay = 1f;
    const float AverageCoinLifeTime = 60f;

    [SerializeField]
    Transform m_topLeftSpawnLimitTransform;
    [SerializeField]
    Transform m_bottomRightSpawnLimitTransform;
    [SerializeField]
    GameObject m_coinPrefab;

    float m_nextSpawnDelay;

    private void Awake() {
        m_nextSpawnDelay = SpawnDelay;
    }

    public override void OnStartServer() {
        base.OnStartServer();

        SpawnRandomCoin(InitialSpawnCount);
    }

    private void Update() {
        if(isServer) {
            m_nextSpawnDelay -= Time.deltaTime;

            if(m_nextSpawnDelay < 0) {
                SpawnRandomCoin();

                m_nextSpawnDelay = 0.5f;
            }
        }
    }

    void SpawnRandomCoin(int amount = 1) {
        for(int i = 0 ; i < amount ; ++i) {
            Vector3 position;
            position.x = Random.Range(m_topLeftSpawnLimitTransform.position.x, m_bottomRightSpawnLimitTransform.position.x);
            position.y = Random.Range(m_topLeftSpawnLimitTransform.position.y, m_bottomRightSpawnLimitTransform.position.y);
            position.z = -1;

            GameObject coin = Instantiate(m_coinPrefab, position, Quaternion.identity, transform);
            float randomRange = 0.5f;
            Destroy(coin, Random.Range(AverageCoinLifeTime * randomRange, AverageCoinLifeTime * (1 + randomRange)));
            NetworkServer.Spawn(coin);
        }
    }
}
