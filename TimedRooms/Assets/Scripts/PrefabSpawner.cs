using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.Networking;

public class PrefabSpawner : NetworkBehaviour {
    public float m_initialDelay;
    public float m_spawnIntervalTime;
    public int m_spawnCount;
    public float m_averageLifeTime;
    public float m_randomRange;

    [SerializeField]
    Transform m_topLeftSpawnLimitTransform;
    [SerializeField]
    Transform m_bottomRightSpawnLimitTransform;
    [SerializeField]
    GameObject[] m_prefabs;
    [SerializeField]
    bool m_isTutoPrefab;

    float m_nextSpawnDelay;

    void Awake() {
        m_nextSpawnDelay = m_initialDelay;
    }

    void Update() {
        if(m_isTutoPrefab == Player.IsTuto) {
            m_nextSpawnDelay -= Time.deltaTime;

            if(m_spawnCount > 0 && m_nextSpawnDelay < 0) {
                SpawnRandomPrefab();
            }
        }
    }

    void SpawnRandomPrefab() {
        Vector3 position;
        int tryCountLeft = 25;
        float emptyDistance = 0.3f;
        bool isValidPosition;
        do {
            isValidPosition = true;
            position.x = Random.Range(m_topLeftSpawnLimitTransform.position.x, m_bottomRightSpawnLimitTransform.position.x);
            position.y = Random.Range(m_topLeftSpawnLimitTransform.position.y, m_bottomRightSpawnLimitTransform.position.y);
            position.z = -1;

            isValidPosition = isValidPosition && !Physics2D.Raycast(position, Vector2.up, emptyDistance);
            isValidPosition = isValidPosition && !Physics2D.Raycast(position, Vector2.right, emptyDistance);
            isValidPosition = isValidPosition && !Physics2D.Raycast(position, Vector2.down, emptyDistance);
            isValidPosition = isValidPosition && !Physics2D.Raycast(position, Vector2.left, emptyDistance);
            isValidPosition = isValidPosition && !Physics2D.Raycast(position, Vector2.up + Vector2.right, emptyDistance);
            isValidPosition = isValidPosition && !Physics2D.Raycast(position, Vector2.right + Vector2.down, emptyDistance);
            isValidPosition = isValidPosition && !Physics2D.Raycast(position, Vector2.down + Vector2.left, emptyDistance);
            isValidPosition = isValidPosition && !Physics2D.Raycast(position, Vector2.left + Vector2.up, emptyDistance);
        } while(!isValidPosition && tryCountLeft-- > 0);

        if(isValidPosition) {
            GameObject newGO = Instantiate(GetRandomPrefab(), position, Quaternion.identity, transform);
            if(m_averageLifeTime > 0) {
                Destroy(newGO, Random.Range(m_averageLifeTime * m_randomRange, m_averageLifeTime * (1 + m_randomRange)));
            }
            GameObjectEvents events = newGO.AddComponent<GameObjectEvents>();
            events.OnDestroyEvent .AddListener(GameObjectDestroyed);
            NetworkServer.Spawn(newGO);
            m_spawnCount--;
            m_nextSpawnDelay = m_spawnIntervalTime;
        } else {
            m_nextSpawnDelay = Mathf.Min(0.1f, m_spawnIntervalTime);
            Debug.Log("Invalid position !");
        }
    }

    GameObject GetRandomPrefab() {
        return m_prefabs[Random.Range(0, m_prefabs.Length)];
    }

    void GameObjectDestroyed() {
        m_spawnCount++;
    }
}
