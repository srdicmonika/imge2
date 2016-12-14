using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public int scr;
    public int enrg;
    void OnGUI()
    {

        GUI.Box(new Rect(30, Screen.height/3- 10, 100, 30), "Score: " + scr);

        GUI.Box(new Rect(30, Screen.height * 2 / 3 - 10, 100, 30),"Energy:" + enrg);

    }
}
