using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour {
    const float MinCameraSize = 1.2f;
    const float MaxCameraSize = 6f;

    const float MinCamDistanceCoef = 0.3f;
    const float CameraSpeed = 30f;

    Transform m_playerTransform;
    Camera m_camera;

    float m_cameraSize;

    private void Awake() {
        m_camera = GetComponent<Camera>();
        enabled = false;
        SetCameraSize(MaxCameraSize);
    }

    public void UpdateCameraSize(int score) {
        float[] coinCountSteps = { 25, 35, 20, 20, 30 };
        float[] zoomDeltaSpets = { 2.3f, 1.4f, 0.8f, 0.4f, 0.2f };

        float totalZoom = MaxCameraSize - MinCameraSize;
        float coef, zoomDelta, coinCount;
        float ratio = 1f;

        for(int i = 0 ; i < coinCountSteps.Length ; ++i) {
            coinCount = coinCountSteps[i];
            zoomDelta = zoomDeltaSpets[i];
            coef = zoomDelta / totalZoom / coinCount;
            ratio -= Mathf.Min(coinCount, score) * coef;
            score -= (int)coinCount;

            //Debug.Log("Step " + i + " Ratio = " + ratio + " - Score = " + score);

            if(score <= 0) {
                break;
            }
        }

        SetCameraSize(Mathf.Max(Player.IsTuto ? 0.5f : 0, ratio));
    }

    void SetCameraSize(float ratio) {
        m_cameraSize = MinCameraSize + (MaxCameraSize - MinCameraSize) * ratio;
        m_camera.orthographicSize = m_cameraSize;
    }

    private void Update() {
        bool isElastic = true;

        if(isElastic) {
            Vector3 camPosition = m_camera.transform.position;
            camPosition.z = 0;
            Vector3 playerPosition = m_playerTransform.position;
            playerPosition.z = 0;

            Vector3 direction = playerPosition - camPosition;
            float distance = direction.magnitude;
            direction = direction.normalized;

            float minCameraDistance = MinCamDistanceCoef * m_camera.orthographicSize;
            if(distance > minCameraDistance) {
                SetCameraPosition(m_camera.transform.position + direction * Time.deltaTime * CameraSpeed * (distance - minCameraDistance) / m_camera.orthographicSize);
            }
        } else {
            SetCameraPosition(m_playerTransform.position);
        }
    }

    public void SetPlayer(Transform playerTransform) {
        m_playerTransform = playerTransform;
        SetCameraPosition(m_playerTransform.position);
        enabled = true;
    }

    void SetCameraPosition(Vector3 position) {
        position.z = m_camera.transform.position.z;
        m_camera.transform.position = position;
    }
}
