using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTimer : MonoBehaviour {

    private bool ready;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!ready)
        {
            //call coroutine
            StartCoroutine(ReadyUp());
        }
	}
    public IEnumerator ReadyUp()
    {
        yield return new WaitForSeconds(0.9f);
        ready = true;
    }
    public bool getReady()
    {
        return ready;
    }
    public void setReady(bool n)
    {
        ready = n;
    }
}
