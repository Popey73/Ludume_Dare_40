using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools {
    static public void PrintPing(string ip, string text = "") {
        Ping ping = new Ping(ip);
        while(ping.isDone == false) ;
        Debug.Log("Ping of " + text + "(" + ip + ")  = " + ping.time + " - IsDone + " + ping.isDone);
    }

}
