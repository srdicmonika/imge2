using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

    public CactusController.RVControl DoorKnob;
    public GameObject particles;
    private CactusController cactusController;
    private float state;
    bool alive = false;
    bool reactive = true;
    private ScoreManager scoreManager;


    // Use this for initialization
    void Start () {
        cactusController = FindObjectOfType<CactusController>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }
    //coroutine will smooth out the response time
    IEnumerator response()
    {
        reactive = false;
        yield return new WaitForSeconds(0.4f);
        reactive = true;

    }
	
	// Update is called once per frame
	void Update () {

        if (reactive)
        {
            state = cactusController.getRVValue(DoorKnob);
            //Debug.Log("State: " + state);
            if (state > 0.9f && !alive)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                alive = true;
                StartCoroutine(response());
            }
            if (state <= 0.9f && alive)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                alive = false;
                StartCoroutine(response());
            }
            if (alive)
            {

            }
        }
	}
}
