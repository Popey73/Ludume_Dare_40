using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectEvents : MonoBehaviour {
    public UnityEvent OnDestroyEvent = new UnityEvent();

    void OnDestroy() {
        OnDestroyEvent.Invoke();
    }
}