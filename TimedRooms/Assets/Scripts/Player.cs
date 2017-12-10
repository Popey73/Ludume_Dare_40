using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour {
    static public bool IsTuto = false;

    const float InitialSpeed = 1.5f;
    const float InitialPower= 1f;
    const float InitialFireSpeed= 2f;
    const float InitialFireRange= 3f;
    const float InitialFireRate= 2f;
    const float InitialMaxShield= 5f;

    const float SpeedUpByCoin = 0.003f;
    const float PowerUpByCoin = 0f;
    const float FireSpeedUpByCoin = 0.03f;
    const float FireRangeUpByCoin = 0.02f;
    const float FireRateUpByCoin = 0.01f;
    const float MaxShieldUpByCoin = 0f;

    [SerializeField]
    Transform m_mouseTransform;
    [SerializeField]
    GameObject m_bulletPrefab;
    [SerializeField]
    GameObject m_shieldGO;
    [SerializeField]
    SpriteRenderer m_playerSpriteRendrer;

    Rigidbody2D m_rigidBody;

    //[SyncVar(hook = "OnPlayerVelocityChanged")]
    //Vector2 m_playerVelocity;

    [SyncVar]
    public float m_att;
    [SyncVar]
    public float m_def;
    [SyncVar]
    public float m_maxShield;
    [SyncVar]
    public float m_shield;
    [SyncVar]
    public float m_speed;
    [SyncVar]
    public float m_fireRate; // x/s
    [SyncVar]
    public float m_fireRange;
    [SyncVar]
    public float m_fireSpeed;

    [SyncVar(hook = "OnCoinChanged")]
    public int m_coin;

    float m_fireCooldownTime;

    [SyncVar(hook = "OnShieldActivatedChanged")]
    bool m_shieldActivated;

    ScoreUI m_scoreUI;
    PlayerCamera m_playerCamera;
    public HPObject m_hpObject;
    public float m_spawnTime;

    void Awake() {
        Cursor.visible = false;

        m_rigidBody = gameObject.GetComponent<Rigidbody2D>();

        m_mouseTransform.gameObject.SetActive(false);
        m_coin = -1;
    }

    private void Start() {
        if(NetworkManager.singleton == null || NetworkManager.singleton.isNetworkActive == false) {
            OnStartLocalPlayer();
        }
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        //Debug.Log("############# OnStartLocalPlayer");

        m_mouseTransform.gameObject.SetActive(true);
        m_shieldGO.SetActive(false);
        m_playerCamera = FindObjectOfType<PlayerCamera>();
        m_playerCamera.SetPlayer(transform);
        m_scoreUI = FindObjectOfType<ScoreUI>();
        m_scoreUI.SetPlayer(this);

        m_hpObject = gameObject.GetComponent<HPObject>();

        IsTuto = PlayerPrefs.GetInt("ShowTuto", 1) == 1;
        InitPlayer();
    }

    public void InitPlayer() {
        m_spawnTime = Time.timeSinceLevelLoad;

        m_speed = 1.5f;
        m_fireRate = 2f;
        m_fireSpeed = 2f;
        m_fireRange = 2f;

        m_att = 1;
        m_def = 0;
        m_maxShield = Application.isEditor ? 100 : 1;
        m_shield = m_maxShield;
        m_coin = 0;

        CmdSetCoin(0);
        m_hpObject.Init(10);

        if(IsTuto) {
            Vector3 tutoPosition = -Vector3.one * 100;
            tutoPosition.z = -1;
            transform.position = tutoPosition;
        } else {
            NetworkStartPosition[] startPositions = FindObjectsOfType<NetworkStartPosition>();
            transform.position = startPositions[Random.Range(0, startPositions.Length)].transform.position;
        }

        m_playerCamera.SetPlayer(this.transform);

        if(PlayerPrefs.GetInt("BestScore") >= 200) {
            m_playerSpriteRendrer.color = Color.green;
        }
    }

    void Update() {
        if(!isLocalPlayer) {
            return;
        }

        if(Input.GetKey(KeyCode.LeftControl) && Application.isEditor) {
            if(Input.GetKeyDown(KeyCode.K)) {
                OnDeath();
            }
            if(Input.GetKeyDown(KeyCode.D)) {
                gameObject.GetComponent<HPObject>().AddHP(-1);
            }
            if(Input.GetKeyDown(KeyCode.C)) {
                AddCoin(15);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            MenuManager.GetMenuManager().m_pauseMenu.DisplayMenu(!MenuManager.GetMenuManager().m_pauseMenu.IsDisplayed());
        }

        m_mouseTransform.localScale = Vector3.one * Camera.main.orthographicSize * 0.1f;

        Vector2 direction = Vector2.zero;

        if(Input.GetButton("Up")) {
            direction += Vector2.up;
        }
        if(Input.GetButton("Right")) {
            direction += Vector2.right;
        }
        if(Input.GetButton("Down")) {
            direction += Vector2.down;
        }
        if(Input.GetButton("Left")) {
            direction += Vector2.left;
        }

        if(Input.GetButtonDown("Shield") && m_shield > 0) {
            CmdActiveShield(true);
        }
        if(Input.GetButtonUp("Shield")) {
            CmdActiveShield(false);
        }

        if(m_shieldActivated) {
            m_shield -= Time.deltaTime;

            if(m_shield < 0) {
                m_shieldActivated = false;
                CmdActiveShield(false);
            }
        } else {
            m_shield = Mathf.Min(m_shield + Time.deltaTime * 0.1f, m_maxShield);
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseposition3 = mousePosition;
        mouseposition3.z = -4;
        m_mouseTransform.position = mouseposition3;

        m_fireCooldownTime -= Time.deltaTime;
        if(Input.GetButton("Fire") && m_fireCooldownTime < 0) {
            Vector3 fireDirection = (m_mouseTransform.position - transform.position);
            fireDirection.z = 0;
            fireDirection = fireDirection.normalized;

            CmdFire(transform.position, m_att, m_fireSpeed, m_fireRange, fireDirection);

            m_fireCooldownTime = 1f / m_fireRate;
        }

        Vector2 velocity = direction.normalized * m_speed;
        //CmdChangedPlayerVelocity(velocity);

        if(!m_shieldActivated) {
            m_rigidBody.velocity = velocity;
        } else {
            m_rigidBody.velocity = Vector2.zero;
        }
    }

    //void UpdateServer() {
    //    m_rigidBody.velocity = m_playerVelocity;
    //}

    public void ActiveShield(bool active) {
        m_shieldActivated = active;
        m_shieldGO.SetActive(active);

        if(isLocalPlayer) {
            m_hpObject.m_invincible = active;
        }
    }

    public void OnDeath() {
        if(!isLocalPlayer) {
            return;
        }

        if(IsTuto) {
            InitPlayer();
        } else {
            Vector3 deathPosition = transform.position;
            deathPosition.x = 3000;
            deathPosition.y = 3000;
            transform.position = deathPosition;

            MenuManager.GetMenuManager().m_endGameMenu.DisplayMenu(this);
        }
    }

    public void AddCoin(int amount) {
        CmdSetCoin(m_coin + amount);
    }

    void SetCoin(int value) {
        m_coin = value;

        //Color color = m_playerSpriteRendrer.color;
        //color.b = ratio;
        //color.g = 1 - ratio;
        //m_playerSpriteRendrer.color = color;

        if(!isLocalPlayer) {
            return;
        }

        m_playerCamera.UpdateCameraSize(m_coin);

        if(IsTuto == false) {
            PlayerPrefs.SetInt("BestScore", Mathf.Max(PlayerPrefs.GetInt("BestScore", 0), m_coin));
        }

        int maxCoinUp = IsTuto ? 35 : 150;
        m_speed = InitialSpeed + SpeedUpByCoin * Mathf.Min(maxCoinUp, m_coin);
        m_maxShield = InitialMaxShield + MaxShieldUpByCoin * Mathf.Min(maxCoinUp, m_coin);
        m_fireRange = InitialFireRange + FireRangeUpByCoin * Mathf.Min(maxCoinUp, m_coin);
        m_fireRate = InitialFireRate + FireRateUpByCoin * Mathf.Min(maxCoinUp, m_coin);
        m_fireSpeed = InitialFireSpeed + FireSpeedUpByCoin * Mathf.Min(maxCoinUp, m_coin);
    }

    #region SyncVarChanged
    void OnShieldActivatedChanged(bool active) {
        ActiveShield(active);
    }

    void OnCoinChanged(int amount) {
        SetCoin(amount);
    }
    #endregion

    #region Command
    [Command]
    void CmdFire(Vector3 position, float att, float speed, float range, Vector3 fireDirection) {
        Vector3 bulletPosition = position + fireDirection * 0.1f;
        GameObject bulletGO = Instantiate(m_bulletPrefab, transform.parent);

        bulletGO.transform.position = bulletPosition;
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bullet.Init(att, speed, range, fireDirection, GetComponent<NetworkIdentity>().netId);

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bulletGO.GetComponent<Collider2D>(), true);

        NetworkServer.Spawn(bulletGO);
    }

    [Command]
    void CmdActiveShield(bool active) {
        m_shieldActivated = active;
        m_hpObject.m_invincible = active;
        if(active) {
            m_rigidBody.velocity = Vector2.zero;
        }
    }

    [Command]
    void CmdSetCoin(int value) {
        m_coin = value;
    }

    //    [Command]
    //    void CmdChangedPlayerVelocity(Vector2 velocity) {
    ////        Debug.Log("CmdChangedPlayerVelocity");
    //        //m_playerVelocity = velocity;
    //        m_rigidBody.velocity = velocity;
    //    }
    #endregion //Command
}
