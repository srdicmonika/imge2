using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    //public Rect variables
    public Rect menuButton;
    public Rect img;
    public Texture tex;

	// Use this for initialization
	void Start () {
        img = new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUI.DrawTexture(img, tex);
        if (GUI.Button(menuButton, "Menu"))
            SceneManager.LoadScene(0);
    }
}
