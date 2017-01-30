using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    //public rect variables
    public Rect buttonRect;
    public Rect instructionsRect;
    public Rect creditsRect;
    public Rect img;
    public Texture tex;

	// Use this for initialization
	void Start () {
        //img = new Rect(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(Screen.width, Screen.height));
        img = new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnGUI()
    {
        GUI.DrawTexture(img, tex);

        if (GUI.Button(buttonRect, "Begin!"))
            SceneManager.LoadScene(1);
        if (GUI.Button(instructionsRect, "Instructions"))
            SceneManager.LoadScene(3);
        if (GUI.Button(creditsRect, "Credits"))
            SceneManager.LoadScene(4);
        
    }
}
