using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Analytics;

public class AnalyticsManager {
    static public void SendScore(int coins, int time) {
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("Coins", coins);
        values.Add("Time", time);

        Analytics.CustomEvent("Score", values);
    }
}
