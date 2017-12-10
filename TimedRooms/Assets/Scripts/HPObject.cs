using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class HPObject : NetworkBehaviour {
    [SerializeField]
    bool m_destroOnDeath;

    [SerializeField]
    GameObject m_coinPrefab;
    [SerializeField]
    int m_coinCountOnDeath;
    [SerializeField]
    GameObject m_healPrefab;
    [SerializeField]
    float m_healProbability;

    [SerializeField]
    SpriteRenderer m_lifeFXSpriteRenderer;
    [SerializeField]
    SpriteRenderer m_lifeWarningSpriteRenderer;

    float m_lifeFXDisplayedTime;

    [SyncVar]
    public float m_maxHp;
    [SyncVar(hook = "OnHpChanged")]
    public float m_hp;

    [SyncVar]
    public bool m_invincible;

    float m_spawnInvinsibleTime;

    public UnityEvent OnDeath = new UnityEvent();

    private void Awake() {
        m_lifeFXDisplayedTime = Mathf.Infinity;
    }

    private void Update() {
        m_spawnInvinsibleTime -= Time.deltaTime;

        if(m_lifeFXDisplayedTime != Mathf.Infinity) {
            m_lifeFXDisplayedTime -= Time.deltaTime;

            if(m_lifeFXDisplayedTime < 0) {
                m_lifeFXSpriteRenderer.gameObject.SetActive(false);
            }
        }
    }

    public void Init(float maxHp) {
        CmdInit(maxHp);
    }

    public void SetMaxHP(float maxHp) {
        m_maxHp = maxHp;
    }

    public void AddHP(float value) {
        if((m_spawnInvinsibleTime > 0 || m_invincible) && value <= 0) {
            return;
        }

        CmdSetHp(m_hp + value);
    }

    [Command]
    void CmdInit(float maxHp) {
        m_maxHp = maxHp;
        m_hp = m_maxHp;
        m_invincible = false;
        m_spawnInvinsibleTime = 3f;
    }

    [Command]
    void CmdSetHp(float value) {
        SetHp(value);
    }

    void SetHp(float value) {
        m_hp = Mathf.Min(m_maxHp, value);

        if(m_hp <= 0) {
            OnDeath.Invoke();

            if(m_coinCountOnDeath > 0) {
                Vector3 randomOffset;
                float randomValue = 0.3f;
                for(int i = 0 ; i < m_coinCountOnDeath ; ++i) {
                    randomOffset = new Vector3(Random.Range(-randomValue, randomValue), Random.Range(-randomValue, randomValue), 0f);
                    GameObject coin = Instantiate(m_coinPrefab, transform.position + randomOffset, Quaternion.identity);
                    Destroy(coin, 4f + randomOffset.x/2f);
                    NetworkServer.Spawn(coin);
                }
            }
            if(Random.Range(0f, 1f) <= m_healProbability) {
                GameObject heal = Instantiate(m_healPrefab, transform.position, Quaternion.identity);
                Destroy(heal, 15f);
                NetworkServer.Spawn(heal);
            }

            if(m_destroOnDeath) {
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    public void OnHpChanged(float value) {
        if(m_lifeFXSpriteRenderer != null && m_hp > 0 && value != m_hp) {
            m_lifeFXDisplayedTime = 0.15f;
            Color color = m_hp <= value ? Color.green : Color.red;
            color.a = 0.3f;
            m_lifeFXSpriteRenderer.color =  color;
            m_lifeFXSpriteRenderer.gameObject.SetActive(true);
        }

        if(!isLocalPlayer) {
            return;
        }

        //Debug.Log("On hp changed = " + value + " - Old = " + m_hp);
        SetHp(value);

        if(m_lifeWarningSpriteRenderer != null) {
            float lifeRatio = m_hp / m_maxHp;
            Color color = m_lifeWarningSpriteRenderer.color;
            color.a = (1f - lifeRatio) * 1.1f;
            m_lifeWarningSpriteRenderer.color = color;
        }
    }
}
