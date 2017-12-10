using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mine : SimpleTurret {
    [SerializeField]
    SpriteRenderer m_mineSprite;

    bool m_exploded;

    protected override void Update() {
        if(m_exploded == false) {
            if(m_nextBulletTime < 0f) {
                m_exploded = true;
            }
            if(m_nextBulletTime < 2) {
                m_mineSprite.enabled = (((int)(m_nextBulletTime * 10) & 1) == 0);
            } else if(m_nextBulletTime < 4) {
                m_mineSprite.enabled = (((int)(m_nextBulletTime * 4) & 1) == 0);
            }
        } else {
            m_mineSprite.enabled = true;
        }

        base.Update();
    }

    public override void InitRandom() {
        //Debug.Log("InitRandom Mine");

        m_nextBulletTime = 6f;

        m_bulletSpeed = Random.Range(2.5f, 3.5f);
        m_bulletRange = Random.Range(7f, 10f);
        m_fireAllPositions = true;
        m_fireInterval = Random.Range(0.015f, 0.03f);
        m_rotationSpeed = Random.Range(500f, 700f);

        Destroy(gameObject, m_nextBulletTime + Random.Range(0.2f, 0.3f));
    }
}
